export function init(dotNetObject) {
    window.addEventListener('resize', () => {
        dotNetObject.invokeMethodAsync('SetIsDesktop', window.innerWidth >= 768);
    });
    dotNetObject.invokeMethodAsync('SetIsDesktop', window.innerWidth >= 768);
}
