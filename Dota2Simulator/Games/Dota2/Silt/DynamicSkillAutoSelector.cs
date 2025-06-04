#if DOTA2
#if Silt

using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing.OCR;
using ImageProcessingSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dota2Simulator.Games.Dota2.Silt
{
    #region 核心枚举和记录类型

    /// <summary>
    /// 选择模式
    /// </summary>
    public enum SelectionMode
    {
        Sequential,
        SkipFirstThenSelect
    }

    /// <summary>
    /// 天赋值类型
    /// </summary>
    public enum TalentValueType
    {
        基础值,
        增量,
        倍数,
        上限,
        触发率,
        持续时间,
        冷却时间,
        作用范围,
        间隔值
    }

    /// <summary>
    /// 值限制类型
    /// </summary>
    public enum ValueLimitType
    {
        无限制,
        伤害最大值,
        间隔最小值,
        叠加最大值,
        范围最大值
    }

    /// <summary>
    /// 优先级奖励配置
    /// </summary>
    public readonly record struct PriorityBonus(
        int MinBonus,
        int MiddleBonus,
        int MaxBonus
    )
    {
        public static readonly PriorityBonus Default = new(0, 0, 0);

        /// <summary>
        /// 计算值对应的奖励
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CalculateBonus(double value, double minValue, double maxValue)
        {
            if (Math.Abs(value - minValue) < 0.001) return MinBonus;
            if (Math.Abs(value - maxValue) < 0.001) return MaxBonus;
            return MiddleBonus;
        }
    }

    #endregion

    #region 优化的数据结构

    /// <summary>
    /// 技能配置（使用struct减少内存分配）
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct SkillConfig
    {
        public double BaseValue;
        public double Increment;
        public double Multiplier;
        public double MaxValue;
        public double MinInterval;
        public int MaxStacks;
        public double MaxRange;
        public double CurrentInterval;
        public double CurrentRange;
        public double StartRange;

        public SkillConfig()
        {
            Multiplier = 1;
            MaxValue = double.MaxValue;
            MinInterval = 0.1;
            MaxStacks = 1;
            MaxRange = double.MaxValue;
            CurrentInterval = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double GetLimitValue(ValueLimitType limitType) => limitType switch
        {
            ValueLimitType.伤害最大值 => MaxValue,
            ValueLimitType.间隔最小值 => MinInterval,
            ValueLimitType.叠加最大值 => MaxStacks,
            ValueLimitType.范围最大值 => MaxRange,
            _ => double.MaxValue
        };

        public readonly double CalculateCurrentValue(int currentStacks = 0)
        {
            var value = BaseValue + (Increment * currentStacks);
            value *= Multiplier;
            return Math.Min(value, MaxValue);
        }

        public readonly (double improvement, string details) CalculateImprovement(
            TalentValueType type, double addValue, int count, int duration)
        {
            // 特殊类型不计算收益
            if (type is TalentValueType.作用范围 or TalentValueType.冷却时间 or TalentValueType.持续时间)
                return (0, $"  {type}类型不计算预期收益");

            int actualCount = count > 0 ? count : (duration > 0 ? (int)(duration / CurrentInterval) : 0);
            if (actualCount == 0) return (Math.Abs(addValue), "  使用简单计算");

            return type switch
            {
                TalentValueType.基础值 => (Math.Abs(addValue) * actualCount * Multiplier,
                    $"  |{addValue}| × {actualCount}次 × {Multiplier}倍率"),

                TalentValueType.倍数 => (CalculateTotalValue(actualCount) * (Math.Abs(addValue) / 100),
                    $"  当前总伤害 × {Math.Abs(addValue)}%"),

                _ => (Math.Abs(addValue), "  默认计算")
            };
        }

        private readonly double CalculateTotalValue(int count)
        {
            double total = 0;
            int stacks = 0;

            for (int i = 0; i < count; i++)
            {
                var value = (BaseValue + Increment * stacks) * Multiplier;
                value = Math.Min(value, MaxValue);
                total += value;
                if (stacks < MaxStacks) stacks++;
            }

            return total;
        }
    }

    /// <summary>
    /// 天赋规则（使用class因为需要委托）
    /// </summary>
    public sealed class TalentRule
    {
        public required string SkillImageHandle { get; init; }
        public required string DescriptionPattern { get; init; }
        public required TalentValueType ValueType { get; init; }
        public ValueLimitType LimitType { get; init; } = ValueLimitType.无限制;
        public double MinValue { get; init; } = double.MinValue;
        public double MaxValue { get; init; } = double.MaxValue;
        public int BasePriority { get; init; } = 100;
        public int MaxSelectionCount { get; init; } = int.MaxValue;
        public bool AutoSkip { get; init; }
        public bool IsSpecialSkill { get; init; }
        public PriorityBonus PriorityBonus { get; init; } = PriorityBonus.Default;
        public Func<SkillConfig, double, int>? CalculateDynamicPriority { get; init; }
    }

    /// <summary>
    /// 图片搜索结果
    /// </summary>
    public readonly record struct ImageSearchResult(
        string ImageName,
        Point Position,
        Point PickPoint,
        Point SkipPoint,
        IReadOnlyList<TalentRule> MatchingRules
    )
    {
        public bool HasRule => MatchingRules.Count > 0;
    }

    /// <summary>
    /// 天赋候选项
    /// </summary>
    public sealed record TalentCandidate
    {
        public required ImageSearchResult SearchResult { get; init; }
        public required string SkillName { get; init; }
        public required string Description { get; init; }
        public required string Effect { get; init; }
        public string MinMaxText { get; init; } = "";
        public double ExtractedValue { get; init; }
        public double ExtractedMinValue { get; init; }
        public double ExtractedMaxValue { get; init; } = double.MaxValue;
        public TalentRule? MatchedRule { get; init; }
        public int Priority { get; init; }
        public double ExpectedImprovement { get; init; }
        public string ImprovementDetails { get; init; } = "";
        public bool IsSpecialSkill { get; init; }
        public bool ShouldSelect { get; init; }
        public bool ShouldSkip { get; init; }
    }

    /// <summary>
    /// 天赋队列项
    /// </summary>
    public readonly record struct TalentQueueItem(
        string Name,
        string Description,
        double Value,
        int Priority,
        TalentCandidate Candidate
    );

    #endregion

    #region 天赋队列管理器

    /// <summary>
    /// 天赋队列管理器
    /// </summary>
    public sealed class TalentQueueManager
    {
        private readonly List<TalentQueueItem> _queue = new();
        private readonly Dictionary<int, List<TalentQueueItem>> _priorityGroups = new();

        public IReadOnlyList<TalentQueueItem> Queue => _queue;

        public void Add(TalentCandidate candidate)
        {
            var item = new TalentQueueItem(
                Name: $"{candidate.SkillName}{candidate.ExtractedValue:+#;-#;0}",
                Description: candidate.Description,
                Value: candidate.ExtractedValue,
                Priority: candidate.Priority,
                Candidate: candidate
            );

            _queue.Add(item);

            if (!_priorityGroups.TryGetValue(candidate.Priority, out var group))
            {
                group = new List<TalentQueueItem>();
                _priorityGroups[candidate.Priority] = group;
            }
            group.Add(item);
        }

        public void Sort()
        {
            _queue.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public string GetQueueDisplay()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== 天赋队列 ===");
            sb.AppendLine($"{"名称",-20} {"优先级",8} {"描述"}");
            sb.AppendLine(new string('-', 60));

            foreach (var item in _queue)
            {
                sb.AppendLine($"{item.Name,-20} {item.Priority,8} {item.Description}");
            }

            // 检查相同优先级
            var duplicatePriorities = _priorityGroups.Where(g => g.Value.Count > 1).ToList();
            if (duplicatePriorities.Any())
            {
                sb.AppendLine("\n⚠️ 警告：以下天赋具有相同优先级：");
                foreach (var (priority, items) in duplicatePriorities)
                {
                    sb.AppendLine($"  优先级 {priority}: {string.Join(", ", items.Select(i => i.Name))}");
                }
            }

            return sb.ToString();
        }

        public void Clear()
        {
            _queue.Clear();
            _priorityGroups.Clear();
        }
    }

    #endregion

    #region 天赋选择器

    /// <summary>
    /// 天赋选择器
    /// </summary>
    public sealed class TalentSelector
    {
        private static readonly Regex NumberRegex = new(@"[+\-]?\d+\.?\d*", RegexOptions.Compiled);
        private readonly ConcurrentDictionary<string, int> _selectionCounts = new();
        private readonly Dictionary<string, SkillConfig> _currentConfigs = new();
        private readonly StringBuilder _detailedLogs = new();
        private readonly TalentQueueManager _queueManager = new();

        public record struct SelectionOptions
        {
            public SelectionOptions()
            {
            }

            public int DelayBetweenChecks { get; init; } = 25;
            public int DelayAfterClick { get; init; } = 25;
            public int DelayAfterSelection { get; init; } = 100;
            public double ImageMatchThreshold { get; init; } = 0.9;
            public int MaxSearchResults { get; init; } = 100;
            public int SearchStep { get; init; } = 10;
            public bool EnableLogging { get; init; } = true;
            public bool EnableTTS { get; init; } = true;
            public bool EnableDetailedLogging { get; init; } = true;
        }

        private SelectionOptions _options = new();

        public void SetOptions(SelectionOptions options) => _options = options;

        public SelectionReport ExecuteSelection(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            IReadOnlyDictionary<string, ImageHandle> imageHandles)
        {
            _detailedLogs.Clear();
            _queueManager.Clear();

            LogDetailed($"=== 开始执行 {config.HeroName} 天赋选择 ===");

            var error_message = "";

            try
            {
                InitializeCurrentConfigs(config);

                var Result = config.Mode switch
                {
                    SelectionMode.Sequential => ProcessSequential(gameHandle, config, imageHandles),
                    _ => ProcessSkipFirstThenSelect(gameHandle, config, imageHandles)
                };

                var report = new SelectionReport
                {
                    HeroName = config.HeroName,
                    StartTime = DateTime.Now,
                    Results = Result,
                    EndTime = DateTime.Now,
                    Success = true,
                    Summary = GenerateSummary(Result),
                    DetailedLog = _detailedLogs.ToString(),
                    QueueDisplay = _queueManager.GetQueueDisplay()
                };

                if (_options.EnableTTS)
                    TTS.TTS.Speak($"{config.HeroName}天赋选择完成");

                return report;
            }
            catch (Exception ex)
            {
                LogDetailed($"错误: {ex.Message}\n{ex.StackTrace}");
                error_message = ex.Message;
            }

            var er_report = new SelectionReport
            {
                HeroName = config.HeroName,
                StartTime = DateTime.Now,
                Success = false,
                ErrorMessage = error_message,
            };

            return er_report;
        }

        private List<SelectionResult> ProcessSkipFirstThenSelect(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            IReadOnlyDictionary<string, ImageHandle> imageHandles)
        {
            var results = new List<SelectionResult>();
            var processedPositions = new HashSet<string>();
            int round = 0;

            while (round++ < 10)
            {
                LogDetailed($"\n--- 第 {round} 轮处理 ---");

                var searchResults = FindAllImages(imageHandles, config);
                var newResults = searchResults
                    .Where(sr => !processedPositions.Contains(GetPositionKey(sr.Position)))
                    .ToList();

                if (newResults.Count == 0)
                {
                    LogDetailed("没有找到新的天赋图标，结束处理");
                    break;
                }

                var candidates = new List<TalentCandidate>();
                var toSkipImmediately = new List<ImageSearchResult>();

                // 创建候选项
                foreach (var searchResult in newResults)
                {
                    if (!searchResult.HasRule)
                    {
                        toSkipImmediately.Add(searchResult);
                    }
                    else if (CreateCandidateFromSearchWithoutDescript(searchResult, gameHandle, config))
                    {
                        toSkipImmediately.Add(searchResult);
                    }
                    else if (CreateCandidateFromSearch(searchResult, gameHandle, config) is { } candidate)
                    {
                        candidates.Add(candidate);
                        _queueManager.Add(candidate);
                    }
                }

                LogDetailed($"\n当前跳过项有{toSkipImmediately.Count}");
                LogDetailed($"\n当前候选项有{candidates.Count}");

                // 显示当前队列
                LogDetailed("\n" + _queueManager.GetQueueDisplay());

                // 处理无规则项
                foreach (var searchResult in toSkipImmediately)
                {
                    PerformSkip(searchResult.SkipPoint);
                    processedPositions.Add(GetPositionKey(searchResult.Position));
                    results.Add(CreateSkipResult(searchResult));
                }

                // 评估并选择
                TalentCandidate? selectedCandidate = null;
                var toSkipCandidates = new List<TalentCandidate>();

                foreach (var candidate in candidates)
                {
                    var candidate1 = EvaluateCandidate(candidate, config);

                    if (candidate1.ShouldSkip)
                        toSkipCandidates.Add(candidate1);
                    else if (candidate1.ShouldSelect && (selectedCandidate == null ||
                             candidate.Priority > selectedCandidate.Priority ||
                             (candidate.Priority == selectedCandidate.Priority &&
                              candidate.ExpectedImprovement > selectedCandidate.ExpectedImprovement)))
                    {
                        LogDetailed($"\n{candidate1.SkillName}_{candidate1.Description}设置为当前选择");
                        selectedCandidate = candidate1;
                    }
                }

                // 执行操作
                if (selectedCandidate != null)
                {
                    LogDetailed("\n" + $"存在要选择的{selectedCandidate.SkillName}_{selectedCandidate.Description}");

                    // 跳过其他候选项
                    foreach (var candidate in toSkipCandidates)
                    {
                        PerformSkip(candidate.SearchResult.SkipPoint);
                        processedPositions.Add(GetPositionKey(candidate.SearchResult.Position));
                        results.Add(CreateResult(candidate, false));
                    }

                    // 选择最佳候选项
                    PerformSelection(selectedCandidate.SearchResult.PickPoint);
                    UpdateCurrentConfig(selectedCandidate);
                    processedPositions.Add(GetPositionKey(selectedCandidate.SearchResult.Position));
                    results.Add(CreateResult(selectedCandidate, true));

                    Thread.Sleep(_options.DelayAfterSelection);
                    PerformNewSelect();
                    break;
                }
                else if (toSkipCandidates.Count > 0)
                {
                    foreach (var candidate in toSkipCandidates)
                    {
                        PerformSkip(candidate.SearchResult.SkipPoint);
                        processedPositions.Add(GetPositionKey(candidate.SearchResult.Position));
                        results.Add(CreateResult(candidate, false));
                    }
                }
                else
                {
                    foreach (var candidate in candidates)
                    {
                        processedPositions.Add(GetPositionKey(candidate.SearchResult.Position));
                        results.Add(CreateResult(candidate, false));
                    }
                    break;
                }
            }

            return results;
        }

        private List<SelectionResult> ProcessSequential(
            in ImageHandle gameHandle,
            TalentSelectionConfig config,
            IReadOnlyDictionary<string, ImageHandle> imageHandles)
        {
            var results = new List<SelectionResult>();
            var searchResults = FindAllImages(imageHandles, config);

            foreach (var searchResult in searchResults)
            {
                if (!searchResult.HasRule)
                {
                    PerformSkip(searchResult.SkipPoint);
                    results.Add(CreateSkipResult(searchResult));
                    continue;
                }

                if (CreateCandidateFromSearch(searchResult, gameHandle, config) is not { } candidate)
                    continue;

                _queueManager.Add(candidate);
                candidate = EvaluateCandidate(candidate, config);

                if (candidate.ShouldSelect)
                {
                    PerformSelection(searchResult.PickPoint);
                    UpdateCurrentConfig(candidate);
                    results.Add(CreateResult(candidate, true));
                }
                else if (candidate.ShouldSkip)
                {
                    PerformSkip(searchResult.SkipPoint);
                    results.Add(CreateResult(candidate, false));
                }
            }

            LogDetailed("\n" + _queueManager.GetQueueDisplay());
            return results;
        }

        private List<ImageSearchResult> FindAllImages(
            IReadOnlyDictionary<string, ImageHandle> imageHandles,
            TalentSelectionConfig config)
        {
            var results = new List<ImageSearchResult>();

            foreach (var (name, imageHandle) in imageHandles)
            {
                var positions = GlobalScreenCapture.FindAllImages(
                    imageHandle,
                    _options.ImageMatchThreshold,
                    _options.MaxSearchResults,
                    _options.SearchStep);

                foreach (var position in positions)
                {
                    var matchingRules = config.Rules
                        .Where(r => r.SkillImageHandle == name && !r.AutoSkip)
                        .ToList();

                    results.Add(new ImageSearchResult(
                        ImageName: name,
                        Position: position,
                        PickPoint: new Point(position.X + 41, position.Y + 306),
                        SkipPoint: new Point(position.X + 41, position.Y + 360),
                        MatchingRules: matchingRules
                    ));
                }
            }

            return results;
        }

        private bool CreateCandidateFromSearchWithoutDescript(
            ImageSearchResult searchResult,
            in ImageHandle gameHandle,
            TalentSelectionConfig config)
        {
            if (!PerformOCR(searchResult, gameHandle, out var ocrData))
                return false;

            foreach (var rule in searchResult.MatchingRules)
            {
                if (MatchesDescription(ocrData.Description, rule.DescriptionPattern))
                    return false;
            }

            return true;
        }

        private TalentCandidate? CreateCandidateFromSearch(
            ImageSearchResult searchResult,
            in ImageHandle gameHandle,
            TalentSelectionConfig config)
        {
            if (!PerformOCR(searchResult, gameHandle, out var ocrData))
                return null;

            foreach (var rule in searchResult.MatchingRules)
            {
                if (!MatchesDescription(ocrData.Description, rule.DescriptionPattern))
                    continue;

                var extractedValue = ExtractValue(ocrData.Effect);
                var skillConfig = _currentConfigs.GetValueOrDefault(ocrData.SkillName);

                // 计算优先级
                int priority = rule.BasePriority;

                // 应用优先级奖励
                var bonus = rule.PriorityBonus.CalculateBonus(extractedValue, rule.MinValue, rule.MaxValue);
                priority += bonus;

                // 动态优先级
                if (rule.CalculateDynamicPriority != null)
                {
                    priority = rule.CalculateDynamicPriority(skillConfig, extractedValue);
                }

                // 计算收益
                var (improvement, details) = skillConfig.CalculateImprovement(
                    rule.ValueType,
                    extractedValue,
                    config.CalculationCount,
                    config.CalculationDuration);

                return new TalentCandidate
                {
                    SearchResult = searchResult,
                    SkillName = ocrData.SkillName,
                    Description = ocrData.Description,
                    Effect = ocrData.Effect,
                    MinMaxText = ocrData.MinMaxText,
                    ExtractedValue = extractedValue,
                    ExtractedMinValue = ocrData.MinValue,
                    ExtractedMaxValue = ocrData.MaxValue,
                    MatchedRule = rule,
                    Priority = priority,
                    ExpectedImprovement = improvement,
                    ImprovementDetails = details,
                    IsSpecialSkill = rule.IsSpecialSkill
                };
            }

            return null;
        }

        private readonly record struct OCRData(
            string SkillName,
            string Description,
            string Effect,
            string MinMaxText,
            double MinValue,
            double MaxValue
        );

        private bool PerformOCR(
            ImageSearchResult searchResult,
            in ImageHandle gameHandle,
            out OCRData ocrData)
        {
            ocrData = default;

            try
            {
                var position = searchResult.Position;

                // OCR区域定义
                ReadOnlySpan<Rectangle> normalRects = [
                    new(position.X - 66, position.Y - 48, 212, 42),  // 技能名
                    new(position.X - 66, position.Y + 114, 212, 42), // 描述
                    new(position.X - 66, position.Y + 183, 212, 42), // 效果
                    new(position.X - 66, position.Y + 216, 212, 42)  // Min/Max
                ];

                var skillName = CleanText(PaddleOCR.获取图片文字(in gameHandle, normalRects[0]));
                var description = CleanText(PaddleOCR.获取图片文字(in gameHandle, normalRects[1]));
                var effect = CleanText(PaddleOCR.获取图片文字(in gameHandle, normalRects[2]));
                var minMaxText = PaddleOCR.获取图片文字(in gameHandle, normalRects[3]);

                // 特殊技能处理
                if (string.IsNullOrWhiteSpace(description))
                {
                    var specialRect = new Rectangle(position.X - 66, position.Y + 170, 212, 42);
                    description = CleanText(PaddleOCR.获取图片文字(in gameHandle, specialRect));
                }

                if (string.IsNullOrWhiteSpace(skillName))
                    skillName = searchResult.ImageName;

                var (minValue, maxValue) = ParseMinMaxValues(minMaxText);

                ocrData = new OCRData(skillName, description, effect, minMaxText, minValue, maxValue);
                return true;
            }
            catch (Exception ex)
            {
                LogDetailed($"OCR异常: {ex.Message}");
                return false;
            }
        }

        private TalentCandidate EvaluateCandidate(TalentCandidate candidate, TalentSelectionConfig config)
        {
            if (candidate.MatchedRule == null)
            {
                candidate = candidate with { ShouldSkip = true };
                LogDetailed($"\n{candidate.SkillName}_{candidate.Description} 不存在匹配的规则");
                return candidate;
            }

            var rule = candidate.MatchedRule;
            var key = GetRuleKey(rule);

            // 检查选择次数
            if (_selectionCounts.TryGetValue(key, out int count) && count >= rule.MaxSelectionCount)
            {
                candidate = candidate with { ShouldSkip = true };
                LogDetailed($"\n{candidate.SkillName}_{candidate.Description} 选择次数已到");
                return candidate;
            }

            // 检查值范围 不在不需要排除
            if (candidate.ExtractedValue < rule.MinValue || candidate.ExtractedValue > rule.MaxValue)
            {
                //candidate = candidate with { ShouldSkip = true };
                LogDetailed($"\n{candidate.SkillName}_{candidate.Description} 不在选择区间");
                return candidate;
            }

            // 根据类型决定选择
            var shouldSelect = rule.ValueType switch
            {
                TalentValueType.作用范围 or TalentValueType.冷却时间 or TalentValueType.持续时间
                    => candidate.Priority >= 0,
                _ => candidate.ExpectedImprovement >= 0
            };

            LogDetailed($"\n{candidate.SkillName}_{candidate.Description}" +
                $"是{rule.ValueType}," +
                $"优先级{candidate.Priority}," +
                $"预计增量{candidate.ExpectedImprovement}");

            candidate = candidate with
            {
                ShouldSelect = shouldSelect,
                ShouldSkip = !shouldSelect
            };

            LogDetailed($"\n{candidate.SkillName}_{candidate.Description}" +
                $"是否选择{candidate.ShouldSelect}," +
                $"是否跳过{candidate.ShouldSkip}");

            return candidate;
        }

        private void UpdateCurrentConfig(TalentCandidate candidate)
        {
            var rule = candidate.MatchedRule!;
            var key = GetRuleKey(rule);

            _selectionCounts.AddOrUpdate(key, 1, (_, count) => count + 1);

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
                    case TalentValueType.间隔值:
                        config.CurrentInterval = Math.Max(config.MinInterval,
                            config.CurrentInterval - Math.Abs(candidate.ExtractedValue));
                        break;
                    case TalentValueType.作用范围:
                        config.CurrentRange = Math.Min(config.MaxRange,
                            config.CurrentRange + Math.Abs(candidate.ExtractedValue));
                        break;
                }
                _currentConfigs[candidate.SkillName] = config;
            }
        }

        private void InitializeCurrentConfigs(TalentSelectionConfig config)
        {
            _currentConfigs.Clear();
            _selectionCounts.Clear();

            foreach (var (name, skillConfig) in config.SkillConfigs)
            {
                _currentConfigs[name] = new SkillConfig
                {
                    BaseValue = skillConfig.BaseValue,
                    Increment = skillConfig.Increment,
                    Multiplier = skillConfig.Multiplier,
                    MaxValue = skillConfig.MaxValue,
                    MinInterval = skillConfig.MinInterval,
                    MaxStacks = skillConfig.MaxStacks,
                    MaxRange = skillConfig.MaxRange,
                    CurrentInterval = skillConfig.CurrentInterval,
                    CurrentRange = skillConfig.CurrentRange,
                    StartRange = skillConfig.StartRange
                };
            }
        }

        // 辅助方法
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool MatchesDescription(string description, string pattern)
            => string.IsNullOrEmpty(pattern) || description.Contains(pattern);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string CleanText(string text)
            => string.IsNullOrEmpty(text) ? "" : Regex.Replace(text, @"[^\w\u4e00-\u9fa5+\-\.\d\s]", "").Trim();

        private static double ExtractValue(string effect)
        {
            var matches = NumberRegex.Matches(effect);
            return matches.Count > 0 && double.TryParse(matches[0].Value, out double value)
                ? Math.Abs(value) : 0;
        }

        private static (double min, double max) ParseMinMaxValues(string minMaxText)
        {
            if (string.IsNullOrEmpty(minMaxText))
                return (0, double.MaxValue);

            double minValue = 0;
            double maxValue = double.MaxValue;

            var minMatch = Regex.Match(minMaxText, @"Min\s*Value:\s*([\d.]+)");
            var maxMatch = Regex.Match(minMaxText, @"Max\s*Value:\s*([\d.]+)");

            if (minMatch.Success)
                double.TryParse(minMatch.Groups[1].Value, out minValue);

            if (maxMatch.Success)
                double.TryParse(maxMatch.Groups[1].Value, out maxValue);

            return (minValue, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRuleKey(TalentRule rule)
            => $"{rule.SkillImageHandle}_{rule.DescriptionPattern}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPositionKey(Point position)
            => $"{position.X},{position.Y}";

        private void PerformSelection(Point position)
        {
            SimKeyBoard.MouseMove(position);
            Thread.Sleep(_options.DelayBetweenChecks);
            SimKeyBoard.MouseLeftClick();
            Thread.Sleep(_options.DelayAfterClick);
        }

        private void PerformSkip(Point position)
        {
            SimKeyBoard.MouseMove(position);
            Thread.Sleep(_options.DelayBetweenChecks);
            SimKeyBoard.MouseLeftClick();
            Thread.Sleep(_options.DelayAfterClick);
        }

        private void PerformNewSelect()
        {
            SimKeyBoard.MouseMove(new Point(1550, 1044));
            Thread.Sleep(_options.DelayBetweenChecks);
            SimKeyBoard.MouseLeftClick();
            Thread.Sleep(_options.DelayAfterClick);
        }

        private static SelectionResult CreateResult(TalentCandidate candidate, bool wasSelected)
            => new()
            {
                Position = candidate.SearchResult.Position,
                SkillName = candidate.SkillName,
                Description = candidate.Description,
                Effect = candidate.Effect,
                MinMaxText = candidate.MinMaxText,
                WasSelected = wasSelected,
                ExtractedValue = candidate.ExtractedValue,
                Priority = candidate.Priority,
                ExpectedImprovement = candidate.ExpectedImprovement,
                IsSpecialSkill = candidate.IsSpecialSkill,
                RuleName = candidate.MatchedRule?.DescriptionPattern ?? "无规则"
            };

        private static SelectionResult CreateSkipResult(ImageSearchResult searchResult)
            => new()
            {
                Position = searchResult.Position,
                SkillName = searchResult.ImageName,
                Description = "未配置规则",
                WasSelected = false,
                RuleName = "无规则"
            };

        private static string GenerateSummary(List<SelectionResult> results)
        {
            var selected = results.Where(r => r.WasSelected).ToList();
            var skipped = results.Where(r => !r.WasSelected).ToList();

            var sb = new StringBuilder();
            sb.AppendLine($"选择了 {selected.Count} 个天赋，跳过了 {skipped.Count} 个天赋。");

            if (selected.Count > 0)
            {
                sb.AppendLine("\n已选择的天赋：");
                foreach (var result in selected)
                {
                    sb.AppendLine($"- {result.SkillName}: {result.Description} (+{result.ExtractedValue})");
                }
            }

            return sb.ToString();
        }

        private void LogDetailed(string message)
        {
            if (_options.EnableDetailedLogging)
            {
                _detailedLogs.AppendLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }

            if (_options.EnableLogging)
            {
                Console.WriteLine($"[天赋选择] {message}");
            }
        }
    }

    #endregion

    #region 结果和配置类

    /// <summary>
    /// 选择结果
    /// </summary>
    public sealed record SelectionResult
    {
        public Point Position { get; init; }
        public string SkillName { get; init; } = "";
        public string Description { get; init; } = "";
        public string Effect { get; init; } = "";
        public string MinMaxText { get; init; } = "";
        public bool WasSelected { get; init; }
        public double ExtractedValue { get; init; }
        public int Priority { get; init; }
        public double ExpectedImprovement { get; init; }
        public bool IsSpecialSkill { get; init; }
        public string RuleName { get; init; } = "";
    }

    /// <summary>
    /// 选择报告
    /// </summary>
    public sealed class SelectionReport
    {
        public required string HeroName { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public List<SelectionResult> Results { get; init; } = new();
        public string Summary { get; init; } = "";
        public string DetailedLog { get; init; } = "";
        public string QueueDisplay { get; init; } = "";

        public TimeSpan Duration => EndTime - StartTime;

        public string ToDetailedString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {HeroName} 天赋选择报告 ===");
            sb.AppendLine($"执行时间: {StartTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"耗时: {Duration.TotalSeconds:F2}秒");
            sb.AppendLine($"状态: {(Success ? "成功" : $"失败 - {ErrorMessage}")}");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(QueueDisplay))
            {
                sb.AppendLine(QueueDisplay);
                sb.AppendLine();
            }

            if (Results.Count > 0)
            {
                sb.AppendLine("【选择详情】");
                foreach (var result in Results)
                {
                    sb.AppendLine($"• {result.SkillName} - {result.Description} [{result.RuleName}]");
                    sb.AppendLine($"  效果: {result.Effect}");
                    if (!string.IsNullOrEmpty(result.MinMaxText))
                    {
                        sb.AppendLine($"  限制: {result.MinMaxText}");
                    }
                    sb.AppendLine($"  状态: {(result.WasSelected ? "✓ 已选择" : "✗ 已跳过")}");
                    sb.AppendLine($"  提取值: {result.ExtractedValue}");
                    sb.AppendLine($"  优先级: {result.Priority}");
                    if (result.ExpectedImprovement > 0)
                    {
                        sb.AppendLine($"  预期收益: {result.ExpectedImprovement:F0}");
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

    /// <summary>
    /// 天赋选择配置
    /// </summary>
    public sealed class TalentSelectionConfig
    {
        public required string HeroName { get; init; }
        public SelectionMode Mode { get; init; } = SelectionMode.SkipFirstThenSelect;
        public IReadOnlyDictionary<string, SkillBaseConfig> SkillConfigs { get; init; } =
            new Dictionary<string, SkillBaseConfig>();
        public List<TalentRule> Rules { get; init; } = new List<TalentRule>();
        public int CalculationDuration { get; init; }
        public int CalculationCount { get; init; }
    }

    /// <summary>
    /// 技能基础配置（兼容旧版）
    /// </summary>
    public sealed class SkillBaseConfig
    {
        public double BaseValue { get; set; }
        public double Increment { get; set; }
        public double Multiplier { get; set; } = 1;
        public double MaxValue { get; set; } = double.MaxValue;
        public double MinInterval { get; set; } = 0.1;
        public int MaxStacks { get; set; } = 1;
        public double MaxRange { get; set; } = double.MaxValue;
        public double CurrentInterval { get; set; } = 1;
        public double CurrentRange { get; set; }
        public double StartRange { get; set; }
    }

    #endregion

    #region 配置构建器

    /// <summary>
    /// 天赋配置构建器
    /// </summary>
    public sealed class TalentConfigBuilder
    {
        private readonly string _heroName;
        private SelectionMode _mode = SelectionMode.SkipFirstThenSelect;
        private readonly Dictionary<string, SkillBaseConfig> _skillConfigs = new();
        private readonly List<TalentRule> _rules = new();
        private readonly Dictionary<string, ImageHandle> _imageHandles = new();
        private int _calculationDuration;
        private int _calculationCount;

        public TalentConfigBuilder(string heroName)
        {
            _heroName = heroName;
        }

        public TalentConfigBuilder SetMode(SelectionMode mode)
        {
            _mode = mode;
            return this;
        }

        public TalentConfigBuilder SetCalculation(int duration = 0, int count = 0)
        {
            _calculationDuration = duration;
            _calculationCount = count;
            return this;
        }

        public TalentConfigBuilder AddSkillConfig(string skillName, Action<SkillBaseConfig> configure)
        {
            var config = new SkillBaseConfig();
            configure(config);
            _skillConfigs[skillName] = config;
            return this;
        }

        public TalentConfigBuilder AddImageHandle(string skillName, ImageHandle handle)
        {
            _imageHandles[skillName] = handle;
            return this;
        }

        public TalentConfigBuilder AddRule(
            string skillImageHandle,
            string descriptionPattern,
            TalentValueType valueType,
            ValueLimitType limitType = ValueLimitType.无限制,
            double minValue = double.MinValue,
            double maxValue = double.MaxValue,
            int basePriority = 100,
            int maxSelectionCount = int.MaxValue,
            bool isSpecialSkill = false,
            PriorityBonus? priorityBonus = null)
        {
            var rule = new TalentRule
            {
                SkillImageHandle = skillImageHandle,
                DescriptionPattern = descriptionPattern,
                ValueType = valueType,
                LimitType = limitType,
                MinValue = minValue,
                MaxValue = maxValue,
                BasePriority = basePriority,
                MaxSelectionCount = maxSelectionCount,
                IsSpecialSkill = isSpecialSkill,
                PriorityBonus = priorityBonus ?? PriorityBonus.Default
            };

            _rules.Add(rule);
            return this;
        }

        public TalentConfigBuilder AddRuleWithBonus(
            string skillImageHandle,
            string descriptionPattern,
            TalentValueType valueType,
            double minValue,
            double maxValue,
            int basePriority,
            int minBonus,
            int middleBonus,
            int maxBonus,
            ValueLimitType limitType = ValueLimitType.无限制,
            int maxSelectionCount = int.MaxValue,
            bool isSpecialSkill = false)
        {
            return AddRule(
                skillImageHandle,
                descriptionPattern,
                valueType,
                limitType,
                minValue,
                maxValue,
                basePriority,
                maxSelectionCount,
                isSpecialSkill,
                new PriorityBonus(minBonus, middleBonus, maxBonus));
        }

        public TalentConfigBuilder AddSkipRule(string skillImageHandle, string descriptionPattern)
        {
            var rule = new TalentRule
            {
                SkillImageHandle = skillImageHandle,
                DescriptionPattern = descriptionPattern,
                ValueType = TalentValueType.基础值,
                AutoSkip = true
            };

            _rules.Add(rule);
            return this;
        }

        public (TalentSelectionConfig config, FrozenDictionary<string, ImageHandle> imageHandles) Build()
        {
            var config = new TalentSelectionConfig
            {
                HeroName = _heroName,
                Mode = _mode,
                SkillConfigs = _skillConfigs.ToFrozenDictionary(),
                Rules = _rules,
                CalculationDuration = _calculationDuration,
                CalculationCount = _calculationCount
            };

            return (config, _imageHandles.ToFrozenDictionary());
        }
    }

    #endregion

    #region 使用示例

    /// <summary>
    /// 英雄配置管理器
    /// </summary>
    public static class HeroConfigManager
    {
        private static readonly ConcurrentDictionary<string, Lazy<TalentConfigBuilder>> _heroConfigs = new();

        static HeroConfigManager()
        {
            RegisterHeroConfig("沙王", CreateSandKingConfig);
            RegisterHeroConfig("钢背兽", CreateBristlebackConfig);
        }

        public static void RegisterHeroConfig(string heroName, Func<TalentConfigBuilder> configFactory)
        {
            _heroConfigs[heroName] = new Lazy<TalentConfigBuilder>(configFactory);
        }

        public static TalentConfigBuilder GetHeroConfig(string heroName)
        {
            if (_heroConfigs.TryGetValue(heroName, out var lazyConfig))
            {
                return lazyConfig.Value;
            }
            throw new InvalidOperationException($"未找到英雄 {heroName} 的配置");
        }

        /// <summary>
        /// 创建沙王配置（使用优先级奖励）
        /// </summary>
        private static TalentConfigBuilder CreateSandKingConfig()
        {
            return new TalentConfigBuilder("沙王")
                .SetMode(SelectionMode.SkipFirstThenSelect)
                .SetCalculation(duration: 0, count: 12)

                .AddSkillConfig("地震", config =>
                {
                    config.BaseValue = 60;
                    config.CurrentInterval = 7;
                    config.MinInterval = 0.1;
                    config.MaxRange = 2700;
                })
                .AddSkillConfig("沙尘暴", config =>
                {
                    config.StartRange = 300;
                    config.CurrentRange = 300;
                    config.MaxRange = 1000;
                })

                .AddImageHandle("先天", Dota2_Pictrue.Silt.先天)
                .AddImageHandle("掘地穿刺", Dota2_Pictrue.Silt.掘地穿刺)
                .AddImageHandle("沙尘暴", Dota2_Pictrue.Silt.沙尘暴)
                .AddImageHandle("尾刺", Dota2_Pictrue.Silt.尾刺)
                .AddImageHandle("地震", Dota2_Pictrue.Silt.地震)

                // 使用优先级奖励
                .AddRuleWithBonus("地震", "碎片胍街間隔", TalentValueType.间隔值,
                    minValue: 3, maxValue: 4, basePriority: 200,
                    minBonus: -10, middleBonus: 0, maxBonus: 10,
                    limitType: ValueLimitType.间隔最小值, maxSelectionCount: 2)

                .AddRuleWithBonus("地震", "每波伤害", TalentValueType.基础值,
                    minValue: 45, maxValue: 45, basePriority: 120,
                    minBonus: 0, middleBonus: 0, maxBonus: 30,
                    maxSelectionCount: 9999)

                .AddRuleWithBonus("地震", "碎片凰", TalentValueType.作用范围,
                    minValue: 475, maxValue: 500, basePriority: 90,
                    minBonus: -10, middleBonus: 15, maxBonus: 20,
                    limitType: ValueLimitType.范围最大值, maxSelectionCount: 6);
        }

        /// <summary>
        /// 创建钢背兽配置
        /// </summary>
        private static TalentConfigBuilder CreateBristlebackConfig()
        {
            return new TalentConfigBuilder("钢背兽")
                .SetMode(SelectionMode.SkipFirstThenSelect)
                .SetCalculation(duration: 10, count: 0)

                .AddSkillConfig("钢毛后背", config =>
                {
                    config.BaseValue = 85;
                    config.Increment = 2;
                    config.MaxStacks = 12;
                    config.MaxValue = 500;
                    config.CurrentInterval = 0.1;
                    config.MinInterval = 0.05;
                })

                .AddImageHandle("钢毛后背", Dota2_Pictrue.Silt.钢毛后背)

                .AddRuleWithBonus("钢毛后背", "每层效果", TalentValueType.增量,
                    minValue: 1.5, maxValue: 3, basePriority: 100,
                    minBonus: -10, middleBonus: 5, maxBonus: 15,
                    limitType: ValueLimitType.伤害最大值, maxSelectionCount: 5)

                .AddSkipRule("钢毛后背", "每层叠加攻击力加成");
        }
    }

    /// <summary>
    /// 使用示例
    /// </summary>
    public static class TalentSelectionExamples
    {
        private static readonly TalentSelector Selector = new();

        public static void ExecuteHeroSelection(string heroName, in ImageHandle gameHandle)
        {
            var builder = HeroConfigManager.GetHeroConfig(heroName);
            var (config, handles) = builder.Build();

            Selector.SetOptions(new TalentSelector.SelectionOptions
            {
                EnableLogging = true,
                EnableTTS = true,
                EnableDetailedLogging = true,
                DelayAfterSelection = 150
            });

            var report = Selector.ExecuteSelection(in gameHandle, config, handles);
            ShowResults(report);
        }

        private static void ShowResults(SelectionReport report)
        {
            var resultText = report.ToDetailedString();
            resultText += report.DetailedLog;

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