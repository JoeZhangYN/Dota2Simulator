## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
DS0001  | Usage    | Warning  | Heroes/ 内禁直调 `_item.根据按键判断技能释放前通用逻辑` (HeroPlan.DispatchAsync 自动调).
DS0002  | Usage    | Warning  | HeroStrategyBase 子类禁 override OnKeyAsync (除非 `[SkipDslDispatch]` escape-hatch).
DS0003  | Usage    | Warning  | Application/+Heroes/ 业务侧禁 `GlobalScreenCapture.GetCurrentHandle()` 直调 (白名单: HeroLoopHost/Silt 透传/SaveImage). 改 `_vision.PixelAt` / `_vision.WithFrame` 端口.
