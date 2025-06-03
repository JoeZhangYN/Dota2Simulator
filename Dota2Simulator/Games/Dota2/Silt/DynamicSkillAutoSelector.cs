#if DOTA2
#if Silt

using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing.Ocr;
using ImageProcessingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dota2Simulator.Games.Dota2.Silt
{
    #region 枚举定义

    /// <summary>
    /// 选择模式
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 原逻辑 - 选择不刷新，跳过也不刷新
        /// </summary>
        Sequential,

        /// <summary>
        /// 实际逻辑 - 先跳过所有其他天赋，再选择
        /// </summary>
        SkipFirstThenSelect
    }

    /// <summary>
    /// 天赋值类型
    /// </summary>
    public enum TalentValueType
    {
        /// <summary>
        /// 基础值 - 直接增加
        /// </summary>
        基础值,

        /// <summary>
        /// 增量 - 每次/每层增加的值
        /// </summary>
        增量,

        /// <summary>
        /// 倍数 - 百分比加成
        /// </summary>
        倍数,

        /// <summary>
        /// 上限 - 最大值限制
        /// </summary>
        上限,

        /// <summary>
        /// 触发率 - 触发概率
        /// </summary>
        触发率,

        /// <summary>
        /// 持续时间
        /// </summary>
        持续时间,

        /// <summary>
        /// 冷却时间
        /// </summary>
        冷却时间
    }

    #endregion

    #region 核心数据结构

    /// <summary>
    /// 技能基础配置
    /// </summary>
    public class SkillBaseConfig
    {
        public string SkillName { get; set; }
        public double BaseValue { get; set; } = 0;      // 基础值
        public double Increment { get; set; } = 0;       // 每次增量
        public double Multiplier { get; set; } = 1;      // 倍数
        public double MaxValue { get; set; } = double.MaxValue;  // 上限值
        public double MinValue { get; set; } = 0;        // 下限值
        public int MaxStacks { get; set; } = 1;          // 最大层数
        public double TriggerInterval { get; set; } = 1; // 触发间隔（秒）

        /// <summary>
        /// 计算当前实际值
        /// </summary>
        public double CalculateCurrentValue(int currentStacks = 0)
        {
            var value = BaseValue + (Increment * currentStacks);
            value *= Multiplier;
            return Math.Max(MinValue, Math.Min(value, MaxValue));
        }

        /// <summary>
        /// 计算增加某个值后的收益
        /// </summary>
        public double CalculateImprovement(TalentValueType type, double addValue, double duration = 10)
        {
            switch (type)
            {
                case TalentValueType.基础值:
                    return addValue * Multiplier * (duration / TriggerInterval);

                case TalentValueType.增量:
                    // 计算实际能达到的层数
                    var effectiveStacks = Math.Min(MaxStacks, duration / TriggerInterval);
                    var currentTotal = CalculateTotalDamage(duration);

                    // 创建临时配置计算新值
                    var tempConfig = this.Clone();
                    tempConfig.Increment += addValue;
                    var newTotal = tempConfig.CalculateTotalDamage(duration);

                    return newTotal - currentTotal;

                case TalentValueType.倍数:
                    var currentDamage = CalculateTotalDamage(duration);
                    return currentDamage * (addValue / 100);

                case TalentValueType.上限:
                    // 只有当前值会达到上限时，增加上限才有意义
                    var maxPossibleValue = BaseValue + (Increment * MaxStacks);
                    if (maxPossibleValue > MaxValue)
                    {
                        var tempConfig2 = this.Clone();
                        tempConfig2.MaxValue += addValue;
                        return tempConfig2.CalculateTotalDamage(duration) - CalculateTotalDamage(duration);
                    }
                    return 0;

                default:
                    return addValue;
            }
        }

        /// <summary>
        /// 计算指定时间内的总伤害
        /// </summary>
        public double CalculateTotalDamage(double duration)
        {
            double totalDamage = 0;
            double currentValue = BaseValue;
            int stacks = 0;

            for (double t = 0; t < duration; t += TriggerInterval)
            {
                currentValue = BaseValue + (Increment * stacks);
                currentValue *= Multiplier;
                currentValue = Math.Max(MinValue, Math.Min(currentValue, MaxValue));

                totalDamage += currentValue;

                if (stacks < MaxStacks)
                    stacks++;
            }

            return totalDamage;
        }

        public SkillBaseConfig Clone()
        {
            return new SkillBaseConfig
            {
                SkillName = this.SkillName,
                BaseValue = this.BaseValue,
                Increment = this.Increment,
                Multiplier = this.Multiplier,
                MaxValue = this.MaxValue,
                MinValue = this.MinValue,
                MaxStacks = this.MaxStacks,
                TriggerInterval = this.TriggerInterval
            };
        }
    }

    /// <summary>
    /// 天赋规则
    /// </summary>
    public class TalentRule
    {
        public string SkillImageHandle { get; set; }
        public string DescriptionPattern { get; set; }
        public TalentValueType ValueType { get; set; }
        public double MinValue { get; set; } = double.MinValue;
        public double MaxValue { get; set; } = double.MaxValue;
        public int BasePriority { get; set; } = 100;
        public int MaxSelectionCount { get; set; } = int.MaxValue;
        public bool AutoSkip { get; set; } = false;
        public bool IsSpecialSkill { get; set; } = false;

        /// <summary>
        /// 动态计算优先级
        /// </summary>
        public Func<SkillBaseConfig, double, int> CalculateDynamicPriority { get; set; }
    }

    /// <summary>
    /// 天赋候选项
    /// </summary>
    public class TalentCandidate
    {
        public Point Position { get; set; }
        public Point PickPoint { get; set; }
        public Point SkipPoint { get; set; }
        public string SkillName { get; set; }           // 技能名称（从特殊区域OCR）
        public string Description { get; set; }          // 描述
        public string Effect { get; set; }               // 效果
        public string MinMaxText { get; set; }           // Min/Max 文本
        public double ExtractedValue { get; set; }       // 提取的数值
        public double ExtractedMinValue { get; set; }    // 提取的最小值
        public double ExtractedMaxValue { get; set; }    // 提取的最大值
        public TalentRule MatchedRule { get; set; }      // 匹配的规则
        public int Priority { get; set; }                // 优先级
        public double ExpectedImprovement { get; set; }  // 预期收益
        public bool IsSpecialSkill { get; set; }         // 是否特殊技能
    }

    /// <summary>
    /// 天赋选择配置
    /// </summary>
    public class TalentSelectionConfig
    {
        public string HeroName { get; set; }
        public SelectionMode Mode { get; set; } = SelectionMode.SkipFirstThenSelect;
        public Dictionary<string, SkillBaseConfig> SkillConfigs { get; set; } = new();
        public List<TalentRule> Rules { get; set; } = new();
        public bool SkipUnmatchedByDefault { get; set; } = true;
        public int CalculationDuration { get; set; } = 10; // 计算收益的时长（秒）
    }

    #endregion

    #region 天赋选择器

    /// <summary>
    /// 优化的天赋选择器
    /// </summary>
    public class OptimizedTalentSelector
    {
        private static readonly Regex NumberRegex = new Regex(@"[+\-]?\d+\.?\d*", RegexOptions.Compiled);
        private readonly Dictionary<string, int> _selectionCounts = new();
        private readonly Dictionary<string, SkillBaseConfig> _currentConfigs = new();

        public class SelectionOptions
        {
            public int DelayBetweenChecks { get; set; } = 100;
            public int DelayAfterClick { get; set; } = 300;
            public int DelayAfterSelection { get; set; } = 500; // 选择后额外延迟，等待刷新
            public double ImageMatchThreshold { get; set; } = 0.9;
            public int MaxSearchResults { get; set; } = 100;
            public int SearchStep { get; set; } = 10;
            public bool EnableLogging { get; set; } = true;
            public bool EnableTTS { get; set; } = true;
        }

        private SelectionOptions _options = new();

        public void SetOptions(SelectionOptions options)
        {
            _options = options ?? new SelectionOptions();
        }

        /// <summary>
        /// 执行天赋选择
        /// </summary>
        public SelectionReport ExecuteSelection(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            Dictionary<string, ImageHandle> imageHandles)
        {
            var report = new SelectionReport
            {
                HeroName = config.HeroName,
                StartTime = DateTime.Now
            };

            try
            {
                // 初始化当前配置
                InitializeCurrentConfigs(config);

                if (config.Mode == SelectionMode.Sequential)
                {
                    // 原逻辑：顺序处理
                    report.Results = ProcessSequential(gameHandle, config, imageHandles);
                }
                else
                {
                    // 实际逻辑：先跳过，再选择
                    report.Results = ProcessSkipFirstThenSelect(gameHandle, config, imageHandles);
                }

                report.EndTime = DateTime.Now;
                report.Success = true;
                report.Summary = GenerateSummary(report.Results);

                if (_options.EnableTTS)
                {
                    TTS.TTS.Speak($"{config.HeroName}天赋选择完成");
                }
            }
            catch (Exception ex)
            {
                report.Success = false;
                report.ErrorMessage = ex.Message;
                Log($"错误: {ex.Message}");
            }

            return report;
        }

        /// <summary>
        /// 顺序处理模式
        /// </summary>
        private List<SelectionResult> ProcessSequential(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            Dictionary<string, ImageHandle> imageHandles)
        {
            var results = new List<SelectionResult>();
            var candidates = CollectAllCandidates(in gameHandle, config, imageHandles);

            if (!candidates.Any())
            {
                Log("未找到任何候选项");
                return results;
            }

            // 按优先级排序
            var sortedCandidates = SortCandidates(candidates, config);

            foreach (var candidate in sortedCandidates)
            {
                if (ShouldSelect(candidate, config))
                {
                    PerformSelection(candidate.PickPoint);
                    UpdateCurrentConfig(candidate);
                    results.Add(CreateResult(candidate, true));
                }
                else
                {
                    PerformSkip(candidate.SkipPoint);
                    results.Add(CreateResult(candidate, false));
                }
            }

            return results;
        }

        /// <summary>
        /// 先跳过再选择模式
        /// </summary>
        private List<SelectionResult> ProcessSkipFirstThenSelect(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            Dictionary<string, ImageHandle> imageHandles)
        {
            var results = new List<SelectionResult>();
            var processedPositions = new HashSet<string>();
            TalentCandidate selectedCandidate = null;

            while (true)
            {
                var candidates = CollectAllCandidates(in gameHandle, config, imageHandles);

                // 过滤已处理的位置
                candidates = candidates.Where(c => !processedPositions.Contains(GetPositionKey(c))).ToList();

                if (!candidates.Any())
                    break;

                // 排序并找到最佳候选
                var sortedCandidates = SortCandidates(candidates, config);
                selectedCandidate = null;

                foreach (var candidate in sortedCandidates)
                {
                    if (ShouldSelect(candidate, config))
                    {
                        selectedCandidate = candidate;
                        break;
                    }
                }

                if (selectedCandidate != null)
                {
                    // 先跳过其他所有候选项
                    foreach (var candidate in candidates.Where(c => c != selectedCandidate))
                    {
                        PerformSkip(candidate.SkipPoint);
                        processedPositions.Add(GetPositionKey(candidate));
                        results.Add(CreateResult(candidate, false));
                        Thread.Sleep(50); // 短暂延迟
                    }

                    // 选择最佳候选项
                    PerformSelection(selectedCandidate.PickPoint);
                    UpdateCurrentConfig(selectedCandidate);
                    processedPositions.Add(GetPositionKey(selectedCandidate));
                    results.Add(CreateResult(selectedCandidate, true));

                    // 等待界面刷新
                    Thread.Sleep(_options.DelayAfterSelection);
                }
                else
                {
                    // 没有要选择的，跳过所有
                    foreach (var candidate in candidates)
                    {
                        PerformSkip(candidate.SkipPoint);
                        processedPositions.Add(GetPositionKey(candidate));
                        results.Add(CreateResult(candidate, false));
                    }
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// 收集所有候选项
        /// </summary>
        private List<TalentCandidate> CollectAllCandidates(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            Dictionary<string, ImageHandle> imageHandles)
        {
            var candidates = new List<TalentCandidate>();
            var processedPositions = new HashSet<string>();

            foreach (var rule in config.Rules.Where(r => !r.AutoSkip))
            {
                if (!imageHandles.TryGetValue(rule.SkillImageHandle, out var imageHandle))
                {
                    Log($"未找到图片句柄: {rule.SkillImageHandle}");
                    continue;
                }

                var positions = GlobalScreenCapture.FindAllImagesOptimized(
                    imageHandle,
                    _options.ImageMatchThreshold,
                    _options.MaxSearchResults,
                    _options.SearchStep);

                foreach (var position in positions)
                {
                    var posKey = $"{position.X},{position.Y}";
                    if (processedPositions.Contains(posKey))
                        continue;

                    processedPositions.Add(posKey);

                    var candidate = ExtractCandidate(position, gameHandle, config, rule);
                    if (candidate != null)
                    {
                        candidates.Add(candidate);
                    }
                }
            }

            return candidates;
        }

        /// <summary>
        /// 提取候选项信息
        /// </summary>
        private TalentCandidate ExtractCandidate(
            Point position,
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            TalentRule rule)
        {
            // 定义OCR区域
            Rectangle descRect, effectRect, minMaxRect, skillNameRect;

            if (rule.IsSpecialSkill)
            {
                // 特殊技能区域
                skillNameRect = new Rectangle(position.X - 66, position.Y - 55, 212, 42);
                descRect = new Rectangle(position.X - 66, position.Y + 170, 212, 42);
                effectRect = new Rectangle(position.X - 66, position.Y + 183, 212, 42);
                minMaxRect = new Rectangle(position.X - 66, position.Y + 216, 212, 42);
            }
            else
            {
                // 普通技能区域
                skillNameRect = new Rectangle(position.X - 66, position.Y - 55, 212, 42);
                descRect = new Rectangle(position.X - 66, position.Y + 114, 212, 42);
                effectRect = new Rectangle(position.X - 66, position.Y + 183, 212, 42);
                minMaxRect = new Rectangle(position.X - 66, position.Y + 216, 212, 42);
            }

            var pickPoint = new Point(position.X + 41, position.Y + 306);
            var skipPoint = new Point(position.X + 41, position.Y + 360);

            // OCR识别
            var skillName = PaddleOCR.获取图片文字(in gameHandle, skillNameRect);
            var description = PaddleOCR.获取图片文字(in gameHandle, descRect);
            var effect = PaddleOCR.获取图片文字(in gameHandle, effectRect);
            var minMaxText = PaddleOCR.获取图片文字(in gameHandle, minMaxRect);

            Log($"OCR结果 - 技能:{skillName}, 描述:{description}, 效果:{effect}, Min/Max:{minMaxText}");

            // 检查是否为特殊技能（描述为空）
            if (string.IsNullOrWhiteSpace(description) && !rule.IsSpecialSkill)
            {
                // 尝试作为特殊技能重新识别
                var specialRule = config.Rules.FirstOrDefault(r =>
                    r.SkillImageHandle == rule.SkillImageHandle && r.IsSpecialSkill);

                if (specialRule != null)
                {
                    return ExtractCandidate(position, gameHandle, config, specialRule);
                }
            }

            // 解析Min/Max值
            var (minValue, maxValue) = ParseMinMaxValues(minMaxText);

            // 创建候选项
            var candidate = new TalentCandidate
            {
                Position = position,
                PickPoint = pickPoint,
                SkipPoint = skipPoint,
                SkillName = CleanText(skillName),
                Description = CleanText(description),
                Effect = CleanText(effect),
                MinMaxText = minMaxText,
                ExtractedMinValue = minValue,
                ExtractedMaxValue = maxValue,
                IsSpecialSkill = rule.IsSpecialSkill
            };

            // 检查是否匹配规则
            if (MatchesRule(candidate, rule))
            {
                candidate.MatchedRule = rule;
                candidate.ExtractedValue = ExtractValue(effect);

                // 计算优先级和预期收益
                CalculatePriorityAndImprovement(candidate, config);

                return candidate;
            }

            return null;
        }

        /// <summary>
        /// 解析Min/Max值
        /// </summary>
        private (double min, double max) ParseMinMaxValues(string minMaxText)
        {
            double minValue = 0;
            double maxValue = double.MaxValue;

            if (!string.IsNullOrEmpty(minMaxText))
            {
                var minMatch = Regex.Match(minMaxText, @"Min\s*Value:\s*([\d.]+)");
                var maxMatch = Regex.Match(minMaxText, @"Max\s*Value:\s*([\d.]+)");

                if (minMatch.Success)
                    double.TryParse(minMatch.Groups[1].Value, out minValue);

                if (maxMatch.Success)
                    double.TryParse(maxMatch.Groups[1].Value, out maxValue);
            }

            return (minValue, maxValue);
        }

        /// <summary>
        /// 检查是否匹配规则
        /// </summary>
        private bool MatchesRule(TalentCandidate candidate, TalentRule rule)
        {
            // 检查描述模式
            if (!string.IsNullOrEmpty(rule.DescriptionPattern))
            {
                if (!candidate.Description.Contains(rule.DescriptionPattern))
                    return false;
            }

            // 检查选择次数
            var key = GetRuleKey(rule);
            if (_selectionCounts.TryGetValue(key, out int count))
            {
                if (count >= rule.MaxSelectionCount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 计算优先级和预期收益
        /// </summary>
        private void CalculatePriorityAndImprovement(TalentCandidate candidate, TalentSelectionConfig config)
        {
            var rule = candidate.MatchedRule;
            candidate.Priority = rule.BasePriority;

            // 获取技能配置
            if (_currentConfigs.TryGetValue(candidate.SkillName, out var skillConfig))
            {
                // 计算预期收益
                candidate.ExpectedImprovement = skillConfig.CalculateImprovement(
                    rule.ValueType,
                    candidate.ExtractedValue,
                    config.CalculationDuration);

                // 动态优先级计算
                if (rule.CalculateDynamicPriority != null)
                {
                    candidate.Priority = rule.CalculateDynamicPriority(skillConfig, candidate.ExtractedValue);
                }
                else
                {
                    // 默认动态优先级：基于收益
                    candidate.Priority += (int)(candidate.ExpectedImprovement / 10);
                }

                // 特殊情况：接近上限时优先选择上限提升
                if (rule.ValueType == TalentValueType.增量)
                {
                    var maxPossibleValue = skillConfig.BaseValue +
                        (skillConfig.Increment * skillConfig.MaxStacks);

                    if (maxPossibleValue > skillConfig.MaxValue * 0.8)
                    {
                        // 接近上限，降低增量优先级
                        candidate.Priority -= 20;
                    }
                }
                else if (rule.ValueType == TalentValueType.上限)
                {
                    var maxPossibleValue = skillConfig.BaseValue +
                        (skillConfig.Increment * skillConfig.MaxStacks);

                    if (maxPossibleValue > skillConfig.MaxValue)
                    {
                        // 会达到上限，提高上限优先级
                        candidate.Priority += 30;
                    }
                }
            }
        }

        /// <summary>
        /// 候选项排序
        /// </summary>
        private List<TalentCandidate> SortCandidates(List<TalentCandidate> candidates, TalentSelectionConfig config)
        {
            return candidates
                .OrderByDescending(c => c.Priority)
                .ThenByDescending(c => c.ExpectedImprovement)
                .ThenByDescending(c => Math.Abs(c.ExtractedValue))
                .ToList();
        }

        /// <summary>
        /// 判断是否应该选择
        /// </summary>
        private bool ShouldSelect(TalentCandidate candidate, TalentSelectionConfig config)
        {
            var rule = candidate.MatchedRule;

            // 检查值范围
            if (candidate.ExtractedValue < rule.MinValue ||
                candidate.ExtractedValue > rule.MaxValue)
            {
                return false;
            }

            // 检查Min/Max限制
            if (candidate.ExtractedValue < candidate.ExtractedMinValue ||
                candidate.ExtractedValue > candidate.ExtractedMaxValue)
            {
                // 特殊情况：如果已经接近上限，且这是最后的选择机会
                if (rule.ValueType == TalentValueType.增量 &&
                    _currentConfigs.TryGetValue(candidate.SkillName, out var skillConfig))
                {
                    var remainingToMax = skillConfig.MaxValue -
                        (skillConfig.BaseValue + skillConfig.Increment * skillConfig.MaxStacks);

                    if (remainingToMax > 0 && remainingToMax < candidate.ExtractedValue)
                    {
                        // 虽然不在范围内，但可以填补剩余空间
                        return true;
                    }
                }
                return false;
            }

            // 检查预期收益
            if (candidate.ExpectedImprovement <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 执行选择
        /// </summary>
        private void PerformSelection(Point position)
        {
            SimKeyBoard.MouseMove(position);
            Thread.Sleep(_options.DelayBetweenChecks);
            SimKeyBoard.MouseLeftClick();
            Thread.Sleep(_options.DelayAfterClick);
        }

        /// <summary>
        /// 执行跳过
        /// </summary>
        private void PerformSkip(Point position)
        {
            SimKeyBoard.MouseMove(position);
            Thread.Sleep(_options.DelayBetweenChecks);
            SimKeyBoard.MouseLeftClick();
            Thread.Sleep(_options.DelayAfterClick);
        }

        /// <summary>
        /// 更新当前配置
        /// </summary>
        private void UpdateCurrentConfig(TalentCandidate candidate)
        {
            var rule = candidate.MatchedRule;

            // 更新选择计数
            var key = GetRuleKey(rule);
            if (!_selectionCounts.ContainsKey(key))
                _selectionCounts[key] = 0;
            _selectionCounts[key]++;

            // 更新技能配置
            if (_currentConfigs.TryGetValue(candidate.SkillName, out var config))
            {
                switch (rule.ValueType)
                {
                    case TalentValueType.基础值:
                        config.BaseValue += candidate.ExtractedValue;
                        break;
                    case TalentValueType.增量:
                        config.Increment += candidate.ExtractedValue;
                        break;
                    case TalentValueType.倍数:
                        config.Multiplier *= (1 + candidate.ExtractedValue / 100);
                        break;
                    case TalentValueType.上限:
                        config.MaxValue += candidate.ExtractedValue;
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化当前配置
        /// </summary>
        private void InitializeCurrentConfigs(TalentSelectionConfig config)
        {
            _currentConfigs.Clear();
            _selectionCounts.Clear();

            foreach (var kvp in config.SkillConfigs)
            {
                _currentConfigs[kvp.Key] = kvp.Value.Clone();
            }
        }

        /// <summary>
        /// 辅助方法
        /// </summary>
        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return Regex.Replace(text, @"[^\w\u4e00-\u9fa5+\-\.\d\s]", "").Trim();
        }

        private double ExtractValue(string effect)
        {
            var matches = NumberRegex.Matches(effect);
            if (matches.Count > 0 && double.TryParse(matches[0].Value, out double value))
            {
                return value;
            }
            return 0;
        }

        private string GetRuleKey(TalentRule rule)
        {
            return $"{rule.SkillImageHandle}_{rule.DescriptionPattern}";
        }

        private string GetPositionKey(TalentCandidate candidate)
        {
            return $"{candidate.Position.X},{candidate.Position.Y}";
        }

        private void Log(string message)
        {
            if (_options.EnableLogging)
            {
                Console.WriteLine($"[天赋选择] {message}");
            }
        }

        private SelectionResult CreateResult(TalentCandidate candidate, bool wasSelected)
        {
            return new SelectionResult
            {
                Position = candidate.Position,
                SkillName = candidate.SkillName,
                Description = candidate.Description,
                Effect = candidate.Effect,
                MinMaxText = candidate.MinMaxText,
                WasSelected = wasSelected,
                ExtractedValue = candidate.ExtractedValue,
                Priority = candidate.Priority,
                ExpectedImprovement = candidate.ExpectedImprovement,
                IsSpecialSkill = candidate.IsSpecialSkill
            };
        }

        private string GenerateSummary(List<SelectionResult> results)
        {
            var selected = results.Where(r => r.WasSelected).ToList();
            var skipped = results.Where(r => !r.WasSelected).ToList();

            var sb = new StringBuilder();
            sb.AppendLine($"选择了 {selected.Count} 个天赋，跳过了 {skipped.Count} 个天赋。");

            if (selected.Any())
            {
                sb.AppendLine("\n已选择的天赋：");
                foreach (var result in selected)
                {
                    sb.AppendLine($"- {result.SkillName}: {result.Description} (+{result.ExtractedValue})");
                }
            }

            return sb.ToString();
        }
    }

    #endregion

    #region 结果类

    /// <summary>
    /// 选择结果
    /// </summary>
    public class SelectionResult
    {
        public Point Position { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public string Effect { get; set; }
        public string MinMaxText { get; set; }
        public bool WasSelected { get; set; }
        public double ExtractedValue { get; set; }
        public int Priority { get; set; }
        public double ExpectedImprovement { get; set; }
        public bool IsSpecialSkill { get; set; }
    }

    /// <summary>
    /// 选择报告
    /// </summary>
    public class SelectionReport
    {
        public string HeroName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<SelectionResult> Results { get; set; } = new();
        public string Summary { get; set; }

        public TimeSpan Duration => EndTime - StartTime;

        public string ToDetailedString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {HeroName} 天赋选择报告 ===");
            sb.AppendLine($"执行时间: {StartTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"耗时: {Duration.TotalSeconds:F2}秒");
            sb.AppendLine($"状态: {(Success ? "成功" : $"失败 - {ErrorMessage}")}");
            sb.AppendLine();

            if (Results?.Any() == true)
            {
                sb.AppendLine("【选择详情】");
                foreach (var result in Results)
                {
                    sb.AppendLine($"• {result.SkillName} - {result.Description}");
                    sb.AppendLine($"  效果: {result.Effect}");
                    sb.AppendLine($"  Min/Max: {result.MinMaxText}");
                    sb.AppendLine($"  状态: {(result.WasSelected ? "✓ 已选择" : "✗ 已跳过")}");
                    sb.AppendLine($"  优先级: {result.Priority}");
                    sb.AppendLine($"  预期收益: {result.ExpectedImprovement:F0}");
                    if (result.IsSpecialSkill)
                    {
                        sb.AppendLine($"  特殊技能: 是");
                    }
                    sb.AppendLine();
                }
            }

            if (!string.IsNullOrEmpty(Summary))
            {
                sb.AppendLine($"【总结】\n{Summary}");
            }

            return sb.ToString();
        }
    }

    #endregion

    #region 配置构建器

    /// <summary>
    /// 天赋配置构建器
    /// </summary>
    public class TalentConfigBuilder
    {
        private readonly TalentSelectionConfig _config;
        private readonly Dictionary<string, ImageHandle> _imageHandles = new();

        public TalentConfigBuilder(string heroName)
        {
            _config = new TalentSelectionConfig
            {
                HeroName = heroName,
                Mode = SelectionMode.SkipFirstThenSelect,
                SkipUnmatchedByDefault = true
            };
        }

        /// <summary>
        /// 设置选择模式
        /// </summary>
        public TalentConfigBuilder SetMode(SelectionMode mode)
        {
            _config.Mode = mode;
            return this;
        }

        /// <summary>
        /// 设置计算时长
        /// </summary>
        public TalentConfigBuilder SetCalculationDuration(int seconds)
        {
            _config.CalculationDuration = seconds;
            return this;
        }

        /// <summary>
        /// 添加技能基础配置
        /// </summary>
        public TalentConfigBuilder AddSkillConfig(string skillName, Action<SkillBaseConfig> configure)
        {
            var config = new SkillBaseConfig { SkillName = skillName };
            configure(config);
            _config.SkillConfigs[skillName] = config;
            return this;
        }

        /// <summary>
        /// 添加图片句柄
        /// </summary>
        public TalentConfigBuilder AddImageHandle(string skillName, ImageHandle handle)
        {
            _imageHandles[skillName] = handle;
            return this;
        }

        /// <summary>
        /// 添加天赋规则
        /// </summary>
        public TalentConfigBuilder AddRule(
            string skillImageHandle,
            string descriptionPattern,
            TalentValueType valueType,
            double minValue = double.MinValue,
            double maxValue = double.MaxValue,
            int basePriority = 100,
            int maxSelectionCount = int.MaxValue,
            bool isSpecialSkill = false)
        {
            var rule = new TalentRule
            {
                SkillImageHandle = skillImageHandle,
                DescriptionPattern = descriptionPattern,
                ValueType = valueType,
                MinValue = minValue,
                MaxValue = maxValue,
                BasePriority = basePriority,
                MaxSelectionCount = maxSelectionCount,
                IsSpecialSkill = isSpecialSkill
            };

            _config.Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// 添加自动跳过规则
        /// </summary>
        public TalentConfigBuilder AddSkipRule(string skillImageHandle, string descriptionPattern)
        {
            var rule = new TalentRule
            {
                SkillImageHandle = skillImageHandle,
                DescriptionPattern = descriptionPattern,
                AutoSkip = true
            };

            _config.Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// 构建配置
        /// </summary>
        public (TalentSelectionConfig config, Dictionary<string, ImageHandle> imageHandles) Build()
        {
            return (_config, _imageHandles);
        }
    }

    #endregion

    #region 使用示例

    /// <summary>
    /// 使用示例
    /// </summary>
    public static class TalentSelectionExamples
    {
        private static readonly OptimizedTalentSelector Selector = new();

        /// <summary>
        /// 钢背兽配置示例
        /// </summary>
        public static void 钢背兽自动选择(in ImageHandle gameHandle)
        {
            var builder = new TalentConfigBuilder("钢背兽")
                .SetMode(SelectionMode.SkipFirstThenSelect)
                .SetCalculationDuration(10)
                // 添加技能配置
                .AddSkillConfig("钢毛后背", config =>
                {
                    config.BaseValue = 85;      // 基础伤害
                    config.Increment = 2;        // 每层增加
                    config.MaxStacks = 12;       // 最大层数
                    config.MaxValue = 500;       // 伤害上限
                    config.TriggerInterval = 0.1; // 0.1秒触发一次
                })
                // 添加图片句柄
                .AddImageHandle("钢毛后背", Dota2_Pictrue.Silt.钢毛后背)
                // 添加规则
                .AddRule("钢毛后背", "每层效果", TalentValueType.增量, 1.5, 3, 100, 5)
                .AddRule("钢毛后背", "最大层数", TalentValueType.基础值, 4, 7, 100, 3)
                .AddRule("钢毛后背", "伤害临界值", TalentValueType.上限, 50, 200, 90, 2)
                .AddSkipRule("钢毛后背", "每层叠加攻击力加成");

            var (config, handles) = builder.Build();

            // 设置选项
            Selector.SetOptions(new OptimizedTalentSelector.SelectionOptions
            {
                EnableLogging = true,
                EnableTTS = true,
                DelayAfterSelection = 500
            });

            // 执行选择
            var report = Selector.ExecuteSelection(in gameHandle, config, handles);

            // 显示结果
            ShowResults(report);
        }

#if false

        /// <summary>
        /// 特殊技能示例
        /// </summary>
        public static void 特殊技能选择(in ImageHandle gameHandle)
        {
            var builder = new TalentConfigBuilder("测试英雄")
                .SetMode(SelectionMode.Sequential) // 使用顺序模式
                .AddSkillConfig("特殊技能", config =>
                {
                    config.BaseValue = 100;
                    config.Multiplier = 1.5;
                })
                .AddImageHandle("特殊技能", Dota2_Pictrue.Silt.特殊技能图标)
                .AddRule("特殊技能", "", TalentValueType.基础值, 50, 150, 120, 1, true); // 特殊技能标记

            var (config, handles) = builder.Build();
            var report = Selector.ExecuteSelection(in gameHandle, config, handles);
            ShowResults(report);
        } 
#endif

        private static void ShowResults(SelectionReport report)
        {
            var resultText = report.ToDetailedString();

            // 更新UI
            Common.Main_Form?.Invoke(() =>
            {
                if (Common.Main_Form.tb_阵营 != null)
                {
                    Common.Main_Form.tb_阵营.Text = resultText;
                }
            });

            Console.WriteLine(resultText);
        }
    }

    #endregion
}
#endif
#endif