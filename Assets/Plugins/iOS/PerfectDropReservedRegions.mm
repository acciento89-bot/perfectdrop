#import <UIKit/UIKit.h>

static UIView *PDActiveRootView(void)
{
    UIApplication *application = UIApplication.sharedApplication;
    for (UIScene *scene in application.connectedScenes)
    {
        if (![scene isKindOfClass:UIWindowScene.class])
            continue;

        UIWindowScene *windowScene = (UIWindowScene *)scene;
        if (windowScene.activationState != UISceneActivationStateForegroundActive &&
            windowScene.activationState != UISceneActivationStateForegroundInactive)
            continue;

        UIWindow *fallback = nil;
        for (UIWindow *window in windowScene.windows)
        {
            if (fallback == nil) fallback = window;
            if (window.isKeyWindow)
                return window.rootViewController.view ?: window;
        }

        if (fallback != nil)
            return fallback.rootViewController.view ?: fallback;
    }

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
    UIWindow *window = application.keyWindow ?: application.windows.firstObject;
#pragma clang diagnostic pop
    return window.rootViewController.view ?: window;
}

extern "C"
{
    void PDPlayHaptic(int kind)
    {
        if (@available(iOS 10.0, *))
        {
            dispatch_async(dispatch_get_main_queue(), ^{
                if (kind == 2)
                {
                    UINotificationFeedbackGenerator *generator = [[UINotificationFeedbackGenerator alloc] init];
                    [generator prepare];
                    [generator notificationOccurred:UINotificationFeedbackTypeSuccess];
                    return;
                }

                UIImpactFeedbackStyle style = kind == 1 ? UIImpactFeedbackStyleMedium : UIImpactFeedbackStyleLight;
                UIImpactFeedbackGenerator *generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:style];
                [generator prepare];
                [generator impactOccurred];
            });
        }
    }

    int PDReservedRegionsSupported(void)
    {
        if (@available(iOS 27.1, *))
            return 1;

        return 0;
    }

    int PDGetPrimaryReservedRegion(
        int kind,
        float *x,
        float *y,
        float *width,
        float *height)
    {
        if (x == NULL || y == NULL || width == NULL || height == NULL)
            return 0;

        if (@available(iOS 27.1, *))
        {
            UIView *view = PDActiveRootView();
            if (view == nil || CGRectIsEmpty(view.bounds))
                return 0;

            UIViewReservedRegionKind *regionKind =
                kind == 1
                    ? UIViewReservedRegionKind.divisionRegionKind
                    : UIViewReservedRegionKind.occlusionRegionKind;

            NSArray<UIViewReservedRegion *> *regions =
                [view reservedRegionsOfKind:regionKind
                                    options:UIViewReservedRegionQueryOptionsNone];

            UIViewReservedRegion *largest = nil;
            CGFloat largestArea = 0.0;

            for (UIViewReservedRegion *region in regions)
            {
                if (!region.isActive)
                    continue;

                CGRect frame = CGRectIntersection(region.frame, view.bounds);
                if (CGRectIsNull(frame) || CGRectIsEmpty(frame))
                    continue;

                CGFloat area = frame.size.width * frame.size.height;
                if (area > largestArea)
                {
                    largestArea = area;
                    largest = region;
                }
            }

            if (largest == nil)
                return 0;

            CGRect frame = CGRectIntersection(largest.frame, view.bounds);
            CGRect bounds = view.bounds;

            const CGFloat normalizedX = (CGRectGetMinX(frame) - CGRectGetMinX(bounds)) / CGRectGetWidth(bounds);
            const CGFloat normalizedYFromTop = (CGRectGetMinY(frame) - CGRectGetMinY(bounds)) / CGRectGetHeight(bounds);
            const CGFloat normalizedWidth = CGRectGetWidth(frame) / CGRectGetWidth(bounds);
            const CGFloat normalizedHeight = CGRectGetHeight(frame) / CGRectGetHeight(bounds);

            *x = (float)normalizedX;
            *y = (float)(1.0 - normalizedYFromTop - normalizedHeight);
            *width = (float)normalizedWidth;
            *height = (float)normalizedHeight;
            return 1;
        }

        return 0;
    }
}
