#if DOTA2

using Dota2Simulator.PictureProcessing;
using ImageProcessingSystem;

namespace Dota2Simulator.Games.Dota2
{

    public static class Dota2_Pictrue
    {
        #region 状态buff
        public static class Buff
        {

            public static ImageHandle 大牛_回魂 => LazyImageLoader.GetImage("BUFF.大牛_回魂");
            public static ImageHandle 光法_大招 => LazyImageLoader.GetImage("BUFF.光法_大招");
            public static ImageHandle 小精灵_幽魂 => LazyImageLoader.GetImage("BUFF.小精灵_幽魂");
            public static ImageHandle 小精灵_过载 => LazyImageLoader.GetImage("BUFF.小精灵_过载");
            public static ImageHandle 物品_TP => LazyImageLoader.GetImage("BUFF.物品_TP");
            public static ImageHandle 火猫_无影拳 => LazyImageLoader.GetImage("BUFF.火猫_无影拳");
            public static ImageHandle 小强_大招 => LazyImageLoader.GetImage("BUFF.小强_大招");
            public static ImageHandle 暗影护符 => LazyImageLoader.GetImage("BUFF.物品_暗影护符");
        }
        #endregion

        #region 命石
        public static class 命石
        {

            public static ImageHandle 伐木机_碎木击 => LazyImageLoader.GetImage("命石.伐木机_碎木击");
            public static ImageHandle 伐木机_锯齿轮旋 => LazyImageLoader.GetImage("命石.伐木机_锯齿轮旋");
            public static ImageHandle 海民_酒友 => LazyImageLoader.GetImage("命石.海民_酒友");
            public static ImageHandle 骷髅王_白骨守卫 => LazyImageLoader.GetImage("命石.骷髅王_白骨守卫");

        }
        #endregion

        #region 英雄技能
        public static class 英雄技能
        {
            public static ImageHandle 卡尔_幽灵漫步 => LazyImageLoader.GetImage("技能.卡尔_幽灵漫步");
            public static ImageHandle 卡尔_强袭飓风 => LazyImageLoader.GetImage("技能.卡尔_强袭飓风");
            public static ImageHandle 卡尔_极速冷却 => LazyImageLoader.GetImage("技能.卡尔_极速冷却");
            public static ImageHandle 卡尔_电磁脉冲 => LazyImageLoader.GetImage("技能.卡尔_电磁脉冲");
        }
        #endregion

        #region 播报信息
        public static class 播报信息
        {
            public static ImageHandle 买活 => LazyImageLoader.GetImage("播报.买活");
            public static ImageHandle 塔防标志 => LazyImageLoader.GetImage("播报.塔防标志");
            public static ImageHandle 盾标志 => LazyImageLoader.GetImage("播报.盾标志");
        }

        #endregion

        #region 物品
        public static class 物品
        {
            public static ImageHandle 以太 => LazyImageLoader.GetImage("物品.以太");
            public static ImageHandle 假腿_力量腿 => LazyImageLoader.GetImage("物品.假腿_力量腿");
            public static ImageHandle 假腿_敏捷腿 => LazyImageLoader.GetImage("物品.假腿_敏捷腿");
            public static ImageHandle 假腿_智力腿 => LazyImageLoader.GetImage("物品.假腿_智力腿");
            public static ImageHandle 刃甲 => LazyImageLoader.GetImage("物品.刃甲");
            public static ImageHandle 刷新球 => LazyImageLoader.GetImage("物品.刷新球");
            public static ImageHandle 否决 => LazyImageLoader.GetImage("物品.否决");
            public static ImageHandle 吹风 => LazyImageLoader.GetImage("物品.吹风");
            public static ImageHandle 天堂 => LazyImageLoader.GetImage("物品.天堂");
            public static ImageHandle 奥术鞋 => LazyImageLoader.GetImage("物品.奥术鞋");
            public static ImageHandle 希瓦 => LazyImageLoader.GetImage("物品.希瓦");
            public static ImageHandle 飓风长戟 => LazyImageLoader.GetImage("物品.飓风长戟");
            public static ImageHandle 青莲宝珠 => LazyImageLoader.GetImage("物品.青莲宝珠");
            public static ImageHandle 幻影斧 => LazyImageLoader.GetImage("物品.幻影斧");
            public static ImageHandle 影之灵龛 => LazyImageLoader.GetImage("物品.影之灵龛");
            public static ImageHandle 推推棒 => LazyImageLoader.GetImage("物品.推推棒");
            public static ImageHandle 微光披风 => LazyImageLoader.GetImage("物品.微光披风");
            public static ImageHandle 隐刀 => LazyImageLoader.GetImage("物品.隐刀");
            public static ImageHandle 大隐刀 => LazyImageLoader.GetImage("物品.大隐刀");
            public static ImageHandle 散失 => LazyImageLoader.GetImage("物品.散失");
            public static ImageHandle 散魂 => LazyImageLoader.GetImage("物品.散魂");
            public static ImageHandle 暗影护符 => LazyImageLoader.GetImage("物品.暗影护符");
            public static ImageHandle 永世法衣 => LazyImageLoader.GetImage("物品.永世法衣");
            public static ImageHandle 深渊之刃 => LazyImageLoader.GetImage("物品.深渊之刃");
            public static ImageHandle 雷神之锤 => LazyImageLoader.GetImage("物品.雷神之锤");
            public static ImageHandle 长盾 => LazyImageLoader.GetImage("物品.长盾");
            public static ImageHandle 炎阳纹章 => LazyImageLoader.GetImage("物品.炎阳纹章");
            public static ImageHandle 玲珑心 => LazyImageLoader.GetImage("物品.玲珑心");
            public static ImageHandle 魔棒 => LazyImageLoader.GetImage("物品.魔棒");
            public static ImageHandle 吊坠 => LazyImageLoader.GetImage("物品.吊坠");
            public static ImageHandle 仙草 => LazyImageLoader.GetImage("物品.仙草");
            public static ImageHandle 赤红甲 => LazyImageLoader.GetImage("物品.赤红甲");
            public static ImageHandle 疯狂面具 => LazyImageLoader.GetImage("物品.疯狂面具");
            public static ImageHandle 相位鞋 => LazyImageLoader.GetImage("物品.相位鞋");
            public static ImageHandle 紫苑 => LazyImageLoader.GetImage("物品.紫苑");
            public static ImageHandle 红杖 => LazyImageLoader.GetImage("物品.红杖");
            public static ImageHandle 红杖2 => LazyImageLoader.GetImage("物品.红杖2");
            public static ImageHandle 红杖3 => LazyImageLoader.GetImage("物品.红杖3");
            public static ImageHandle 红杖4 => LazyImageLoader.GetImage("物品.红杖4");
            public static ImageHandle 红杖5 => LazyImageLoader.GetImage("物品.红杖5");
            public static ImageHandle 纷争 => LazyImageLoader.GetImage("物品.纷争");
            public static ImageHandle 被控_纷争 => LazyImageLoader.GetImage("物品.被控_纷争");
            public static ImageHandle 缚灵锁 => LazyImageLoader.GetImage("物品.缚灵锁");
            public static ImageHandle 羊刀 => LazyImageLoader.GetImage("物品.羊刀");
            public static ImageHandle 臂章 => LazyImageLoader.GetImage("物品.臂章");
            public static ImageHandle 臂章_开启 => LazyImageLoader.GetImage("物品.臂章_开启");
            public static ImageHandle 被控_虚灵 => LazyImageLoader.GetImage("物品.被控_虚灵");
            public static ImageHandle 虚灵之刃 => LazyImageLoader.GetImage("物品.虚灵之刃");
            public static ImageHandle 血棘 => LazyImageLoader.GetImage("物品.血棘");
            public static ImageHandle 血精石 => LazyImageLoader.GetImage("物品.血精石");
            public static ImageHandle 跳刀 => LazyImageLoader.GetImage("物品.跳刀");
            public static ImageHandle 跳刀_力量跳刀 => LazyImageLoader.GetImage("物品.跳刀_力量跳刀");
            public static ImageHandle 跳刀_敏捷跳刀 => LazyImageLoader.GetImage("物品.跳刀_敏捷跳刀");
            public static ImageHandle 跳刀_智力跳刀 => LazyImageLoader.GetImage("物品.跳刀_智力跳刀");
            public static ImageHandle 阿托斯之棍 => LazyImageLoader.GetImage("物品.阿托斯之棍");
            public static ImageHandle 陨星锤 => LazyImageLoader.GetImage("物品.陨星锤");
            public static ImageHandle 魂之灵龛 => LazyImageLoader.GetImage("物品.魂之灵龛");
            public static ImageHandle 魂戒 => LazyImageLoader.GetImage("物品.魂戒");
            public static ImageHandle 鱼叉 => LazyImageLoader.GetImage("物品.鱼叉");
            public static ImageHandle 黑皇杖 => LazyImageLoader.GetImage("物品.黑皇杖");

            public static ImageHandle 虚空至宝_雷神之锤 => LazyImageLoader.GetImage("物品.虚空至宝_雷神之锤");
            public static ImageHandle 虚空至宝_疯狂面具 => LazyImageLoader.GetImage("物品.虚空至宝_疯狂面具");

            public static ImageHandle 书 => LazyImageLoader.GetImage("物品.书");

            public static ImageHandle 中立_永恒遗物 => LazyImageLoader.GetImage("物品.中立_永恒遗物");
            public static ImageHandle 中立_祭礼长袍 => LazyImageLoader.GetImage("物品.中立_祭礼长袍");
        }

        #endregion

        #region Silt
#if Silt
        public static class Silt
        {
            public static ImageHandle 选择天赋 => LazyImageLoader.GetImage("Silt.选择天赋");
            public static ImageHandle 金色天赋 => LazyImageLoader.GetImage("Silt.金色天赋");
            public static ImageHandle 普通天赋 => LazyImageLoader.GetImage("Silt.普通天赋");

            #region 钢背

            public static ImageHandle 鼻涕 => LazyImageLoader.GetImage("Silt.钢背.鼻涕");
            public static ImageHandle 刺针扫射 => LazyImageLoader.GetImage("Silt.钢背.刺针扫射");
            public static ImageHandle 钢毛后背 => LazyImageLoader.GetImage("Silt.钢背.钢毛后背");
            public static ImageHandle 毛团 => LazyImageLoader.GetImage("Silt.钢背.毛团");
            public static ImageHandle 战意 => LazyImageLoader.GetImage("Silt.钢背.战意");

            #endregion

            #region 附魔

            #region 1-4

            public static ImageHandle 神秘 => LazyImageLoader.GetImage("Silt.附魔.神秘");
            public static ImageHandle 壮实 => LazyImageLoader.GetImage("Silt.附魔.壮实");
            public static ImageHandle 警觉 => LazyImageLoader.GetImage("Silt.附魔.警觉");
            public static ImageHandle 坚强 => LazyImageLoader.GetImage("Silt.附魔.坚强");
            public static ImageHandle 迅速 => LazyImageLoader.GetImage("Silt.附魔.迅速");

            #endregion

            #region 2-3

            public static ImageHandle 犀利 => LazyImageLoader.GetImage("Silt.附魔.犀利");
            // 高远
            public static ImageHandle 贪婪 => LazyImageLoader.GetImage("Silt.附魔.贪婪");

            #endregion

            #region 2-4

            public static ImageHandle 吸血鬼 => LazyImageLoader.GetImage("Silt.附魔.吸血鬼");

            #endregion

            #region 4-5

            public static ImageHandle 永恒 => LazyImageLoader.GetImage("Silt.附魔.永恒");
            public static ImageHandle 巨神 => LazyImageLoader.GetImage("Silt.附魔.巨神");
            public static ImageHandle 粗暴 => LazyImageLoader.GetImage("Silt.附魔.粗暴");

            #endregion

            #region 5

            public static ImageHandle 狂热 => LazyImageLoader.GetImage("Silt.附魔.狂热");
            public static ImageHandle 捷足 => LazyImageLoader.GetImage("Silt.附魔.捷足");
            public static ImageHandle 冒险 => LazyImageLoader.GetImage("Silt.附魔.冒险");
            public static ImageHandle 进化 => LazyImageLoader.GetImage("Silt.附魔.进化");
            public static ImageHandle 无边 => LazyImageLoader.GetImage("Silt.附魔.无边");
            public static ImageHandle 睿智 => LazyImageLoader.GetImage("Silt.附魔.睿智");

            #endregion

            #endregion
#endif
        }
        #endregion
    }

    // 过去用到的
#if false
    public static class 过去用到的
    {
        #region 用到的所有图片全局变量

        #region 状态buff

        public static readonly ImageHandle Buff_大牛_回魂_handle = 缓存嵌入的图片("BUFF.大牛_回魂");
        public static readonly ImageHandle Buff_光法_大招_handle = 缓存嵌入的图片("BUFF.光法_大招");
        public static readonly ImageHandle Buff_小精灵_幽魂_handle = 缓存嵌入的图片("BUFF.小精灵_幽魂");
        public static readonly ImageHandle Buff_小精灵_过载_handle = 缓存嵌入的图片("BUFF.小精灵_过载");
        public static readonly ImageHandle Buff_物品_TP_handle = 缓存嵌入的图片("BUFF.物品_TP");
        public static readonly ImageHandle Buff_火猫_无影拳_handle = 缓存嵌入的图片("BUFF.火猫_无影拳");
        public static readonly ImageHandle Buff_小强_大招_handle = 缓存嵌入的图片("BUFF.小强_大招");
        public static readonly ImageHandle Buff_暗影护符_handle = 缓存嵌入的图片("BUFF.物品_暗影护符");

        #endregion

        #region 命石

        public static readonly ImageHandle 命石_伐木机_碎木击_handle = 缓存嵌入的图片("命石.伐木机_碎木击");
        public static readonly ImageHandle 命石_伐木机_锯齿轮旋_handle = 缓存嵌入的图片("命石.伐木机_锯齿轮旋");
        public static readonly ImageHandle 命石_海民_酒友_handle = 缓存嵌入的图片("命石.海民_酒友");
        public static readonly ImageHandle 命石_骷髅王_白骨守卫_handle = 缓存嵌入的图片("命石.骷髅王_白骨守卫");

        #endregion

        #region 英雄技能

        public static readonly ImageHandle 技能_卡尔_幽灵漫步_handle = 缓存嵌入的图片("技能.卡尔_幽灵漫步");
        public static readonly ImageHandle 技能_卡尔_强袭飓风_handle = 缓存嵌入的图片("技能.卡尔_强袭飓风");
        public static readonly ImageHandle 技能_卡尔_极速冷却_handle = 缓存嵌入的图片("技能.卡尔_极速冷却");
        public static readonly ImageHandle 技能_卡尔_电磁脉冲_handle = 缓存嵌入的图片("技能.卡尔_电磁脉冲");

        #endregion

        #region 播报信息

        public static readonly ImageHandle 播报_买活_handle = 缓存嵌入的图片("播报.买活");
        public static readonly ImageHandle 播报_塔防标志_handle = 缓存嵌入的图片("播报.塔防标志");
        public static readonly ImageHandle 播报_盾标志_handle = 缓存嵌入的图片("播报.盾标志");

        #endregion

        #region 物品

        public static readonly ImageHandle 物品_以太_handle = 缓存嵌入的图片("物品.以太");
        public static readonly ImageHandle 物品_假腿_力量腿_handle = 缓存嵌入的图片("物品.假腿_力量腿");
        public static readonly ImageHandle 物品_假腿_敏捷腿_handle = 缓存嵌入的图片("物品.假腿_敏捷腿");
        public static readonly ImageHandle 物品_假腿_智力腿_handle = 缓存嵌入的图片("物品.假腿_智力腿");
        public static readonly ImageHandle 物品_刃甲_handle = 缓存嵌入的图片("物品.刃甲");
        public static readonly ImageHandle 物品_刷新球_handle = 缓存嵌入的图片("物品.刷新球");
        public static readonly ImageHandle 物品_否决_handle = 缓存嵌入的图片("物品.否决");
        public static readonly ImageHandle 物品_吹风_handle = 缓存嵌入的图片("物品.吹风");
        public static readonly ImageHandle 物品_天堂_handle = 缓存嵌入的图片("物品.天堂");
        public static readonly ImageHandle 物品_奥术鞋_handle = 缓存嵌入的图片("物品.奥术鞋");
        public static readonly ImageHandle 物品_希瓦_handle = 缓存嵌入的图片("物品.希瓦");
        public static readonly ImageHandle 物品_飓风长戟_handle = 缓存嵌入的图片("物品.飓风长戟");
        public static readonly ImageHandle 物品_青莲宝珠_handle = 缓存嵌入的图片("物品.青莲宝珠");
        public static readonly ImageHandle 物品_幻影斧_handle = 缓存嵌入的图片("物品.幻影斧");
        public static readonly ImageHandle 物品_影之灵龛_handle = 缓存嵌入的图片("物品.影之灵龛");
        public static readonly ImageHandle 物品_推推棒_handle = 缓存嵌入的图片("物品.推推棒");
        public static readonly ImageHandle 物品_微光披风_handle = 缓存嵌入的图片("物品.微光披风");
        public static readonly ImageHandle 物品_隐刀_handle = 缓存嵌入的图片("物品.隐刀");
        public static readonly ImageHandle 物品_大隐刀_handle = 缓存嵌入的图片("物品.大隐刀");
        public static readonly ImageHandle 物品_散失_handle = 缓存嵌入的图片("物品.散失");
        public static readonly ImageHandle 物品_散魂_handle = 缓存嵌入的图片("物品.散魂");
        public static readonly ImageHandle 物品_暗影护符_handle = 缓存嵌入的图片("物品.暗影护符");
        public static readonly ImageHandle 物品_永世法衣_handle = 缓存嵌入的图片("物品.永世法衣");
        public static readonly ImageHandle 物品_深渊之刃_handle = 缓存嵌入的图片("物品.深渊之刃");
        public static readonly ImageHandle 物品_雷神之锤_handle = 缓存嵌入的图片("物品.雷神之锤");
        public static readonly ImageHandle 物品_长盾_handle = 缓存嵌入的图片("物品.长盾");
        public static readonly ImageHandle 物品_炎阳纹章_handle = 缓存嵌入的图片("物品.炎阳纹章");
        public static readonly ImageHandle 物品_玲珑心_handle = 缓存嵌入的图片("物品.玲珑心");
        public static readonly ImageHandle 物品_魔棒_handle = 缓存嵌入的图片("物品.魔棒");
        public static readonly ImageHandle 物品_吊坠_handle = 缓存嵌入的图片("物品.吊坠");
        public static readonly ImageHandle 物品_仙草_handle = 缓存嵌入的图片("物品.仙草");
        public static readonly ImageHandle 物品_赤红甲_handle = 缓存嵌入的图片("物品.赤红甲");
        public static readonly ImageHandle 物品_疯狂面具_handle = 缓存嵌入的图片("物品.疯狂面具");
        public static readonly ImageHandle 物品_相位鞋_handle = 缓存嵌入的图片("物品.相位鞋");
        public static readonly ImageHandle 物品_紫苑_handle = 缓存嵌入的图片("物品.紫苑");
        public static readonly ImageHandle 物品_红杖_handle = 缓存嵌入的图片("物品.红杖");
        public static readonly ImageHandle 物品_红杖2_handle = 缓存嵌入的图片("物品.红杖2");
        public static readonly ImageHandle 物品_红杖3_handle = 缓存嵌入的图片("物品.红杖3");
        public static readonly ImageHandle 物品_红杖4_handle = 缓存嵌入的图片("物品.红杖4");
        public static readonly ImageHandle 物品_红杖5_handle = 缓存嵌入的图片("物品.红杖5");
        public static readonly ImageHandle 物品_纷争_handle = 缓存嵌入的图片("物品.纷争");
        public static readonly ImageHandle 物品_被控_纷争_handle = 缓存嵌入的图片("物品.被控_纷争");
        public static readonly ImageHandle 物品_缚灵锁_handle = 缓存嵌入的图片("物品.缚灵锁");
        public static readonly ImageHandle 物品_羊刀_handle = 缓存嵌入的图片("物品.羊刀");
        public static readonly ImageHandle 物品_臂章_handle = 缓存嵌入的图片("物品.臂章");
        public static readonly ImageHandle 物品_臂章_开启_handle = 缓存嵌入的图片("物品.臂章_开启");
        public static readonly ImageHandle 物品_被控_虚灵_handle = 缓存嵌入的图片("物品.被控_虚灵");
        public static readonly ImageHandle 物品_虚灵之刃_handle = 缓存嵌入的图片("物品.虚灵之刃");
        public static readonly ImageHandle 物品_血棘_handle = 缓存嵌入的图片("物品.血棘");
        public static readonly ImageHandle 物品_血精石_handle = 缓存嵌入的图片("物品.血精石");
        public static readonly ImageHandle 物品_跳刀_handle = 缓存嵌入的图片("物品.跳刀");
        public static readonly ImageHandle 物品_跳刀_力量跳刀_handle = 缓存嵌入的图片("物品.跳刀_力量跳刀");
        public static readonly ImageHandle 物品_跳刀_敏捷跳刀_handle = 缓存嵌入的图片("物品.跳刀_敏捷跳刀");
        public static readonly ImageHandle 物品_跳刀_智力跳刀_handle = 缓存嵌入的图片("物品.跳刀_智力跳刀");
        public static readonly ImageHandle 物品_阿托斯之棍_handle = 缓存嵌入的图片("物品.阿托斯之棍");
        public static readonly ImageHandle 物品_陨星锤_handle = 缓存嵌入的图片("物品.陨星锤");
        public static readonly ImageHandle 物品_魂之灵龛_handle = 缓存嵌入的图片("物品.魂之灵龛");
        public static readonly ImageHandle 物品_魂戒_handle = 缓存嵌入的图片("物品.魂戒");
        public static readonly ImageHandle 物品_鱼叉_handle = 缓存嵌入的图片("物品.鱼叉");
        public static readonly ImageHandle 物品_黑皇杖_handle = 缓存嵌入的图片("物品.黑皇杖");

        public static readonly ImageHandle 物品_虚空至宝_雷神之锤_handle = 缓存嵌入的图片("物品.虚空至宝_雷神之锤");
        public static readonly ImageHandle 物品_虚空至宝_疯狂面具_handle = 缓存嵌入的图片("物品.虚空至宝_疯狂面具");

        public static readonly ImageHandle 物品_书_handle = 缓存嵌入的图片("物品.书");

        public static readonly ImageHandle 中立_永恒遗物_handle = 缓存嵌入的图片("物品.中立_永恒遗物");
        public static readonly ImageHandle 中立_祭礼长袍_handle = 缓存嵌入的图片("物品.中立_祭礼长袍");
        #endregion

        #endregion

        /// <summary>
        /// 缓存对应的句柄
        /// </summary>
        private static Dictionary<string, ImageHandle> _embeddedImageCache = [];

        /// <summary>
        /// 缓存嵌入的图片 - 使用静态图像类型
        /// </summary>
        public static ImageHandle 缓存嵌入的图片(string bpName)
        {
            _embeddedImageCache ??= [];

            // 先检查缓存
            if (_embeddedImageCache.TryGetValue(bpName, out ImageHandle cachedHandle))
            {
                return cachedHandle;
            }

            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 指定嵌入资源的命名空间和文件名
            string resourceName = $"Dota2Simulator.Picture_Dota2.{bpName}.bmp";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using Bitmap bitmap = new(stream);

                // 将Bitmap转换为字节数组
                byte[] imageData = ImageManager.GetBitmapData(bitmap);

                //使用ImageManager创建静态图像句柄（特征图片应该使用静态类型）
                ImageHandle handle = ImageManager.CreateStaticImage(imageData, bitmap.Size, bpName);

                //缓存句柄
                _embeddedImageCache.Add(bpName, handle);

                return handle;
            }
            else
            {
                Tts.Speak($"{bpName}图片不存在");
                // 延迟3秒
                // Delay(3000);
                return ImageHandle.Invalid;
            }
        }

        /// <summary>
        /// 清理缓存的方法
        /// </summary>
        private static void 清理嵌入图片缓存()
        {
            foreach (var kvp in _embeddedImageCache)
            {
                ImageManager.ReleaseImage(kvp.Value);
            }
            _embeddedImageCache.Clear();
        }
    }   
#endif

}
#endif