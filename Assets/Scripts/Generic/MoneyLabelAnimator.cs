using UnityEngine;
using UnityEngine.UIElements;

public class MoneyLabelAnimator
{
    private Label label;
    private int displayed;
    private bool initialized;
    private IVisualElementScheduledItem anim;

    public void Bind(Label label)
    {
        this.label = label;
        initialized = false;
    }

    public void SetImmediate(int value)
    {
        if (label == null) return;
        anim?.Pause();
        displayed = value;
        label.text = value.ToString();
        initialized = true;
    }

    public void AnimateTo(int target)
    {
        if (label == null) return;
        if (!initialized) { SetImmediate(target); return; }

        anim?.Pause();

        int from = displayed;
        int delta = target - from;
        if (delta == 0) return;

        label.RemoveFromClassList("money-gain");
        label.RemoveFromClassList("money-loss");
        label.AddToClassList(delta > 0 ? "money-gain" : "money-loss");

        float duration = Mathf.Clamp(Mathf.Abs(delta) * 0.01f, 0.1f, 0.4f);
        float start = Time.unscaledTime;

        anim = label.schedule.Execute(() =>
        {
            float t = Mathf.Clamp01((Time.unscaledTime - start) / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            displayed = Mathf.RoundToInt(Mathf.Lerp(from, target, eased));
            label.text = displayed.ToString();

            if (t >= 1f)
            {
                anim.Pause();
                displayed = target;
                label.text = target.ToString();
                label.RemoveFromClassList("money-gain");
                label.RemoveFromClassList("money-loss");
            }
        }).Every(16);
    }
}