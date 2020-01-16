using UnityEngine;
using NaughtyAttributes;

public class ProgressBar : MonoBehaviour
{
    [ProgressBar(name: "Health", maxValueFieldName:"", maxValue: 100, color: ProgressBarColor.Orange)]
    public float health = 50;
}
