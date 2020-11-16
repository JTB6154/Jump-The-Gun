using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ContourEdgeOutlineRenderer), PostProcessEvent.AfterStack, "Custom/ContourEdgeOutline")]
public sealed class ContourEdgeOutline : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("The distance that Sobel operator will sample from the central fragment. " +
        "This is not a direct pixel-width outline size.")]
    public FloatParameter thickness = new FloatParameter { value = 1.0f };

    [Range(0f, 1f), Tooltip("A scalar value to multiply with the Sobel value. " +
        "Used to increase or decrease the outline contribution from the depth samples.")]
    public FloatParameter multiplier = new FloatParameter { value = 1.0f };

    [Range(0f, 1f), Tooltip("A scalar value to raise the Sobel value to. " +
        "Extremely useful for removing noise artifacts from the outline.")]
    public FloatParameter bias = new FloatParameter { value = 1.0f };

    [Tooltip("The RGBA color of the outline (where the outline blends with the scene if A is not 1.0)")]
    public ColorParameter color = new ColorParameter { value = Color.black };
}

public sealed class ContourEdgeOutlineRenderer : PostProcessEffectRenderer<ContourEdgeOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ContourEdgeOutline"));

        // Send the parameter values to the shader
        sheet.properties.SetFloat("_OutlineThickness", settings.thickness);
        sheet.properties.SetFloat("_OutlineMultiplier", settings.multiplier);
        sheet.properties.SetFloat("_OutlineBias", settings.bias);
        sheet.properties.SetColor("_OutlineColor", settings.color);

        // Blit a fullscreen pass with the shader to a destination using our source image as an input.
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}