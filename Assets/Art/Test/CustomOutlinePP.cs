using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/CustomOutlinePP")]
public sealed class CustomOutlinePP : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 10f);

    // outline paramaters
    [Tooltip("Thickness of lines")]
    public FloatParameter Thickness = new FloatParameter(2f);

    [Tooltip("Start point")]
    public FloatParameter Edge = new FloatParameter(0.1f);

    [Tooltip("Smoothnes of lines")]
    public FloatParameter Transition = new FloatParameter(0.1f);

    [Tooltip("Colour of lines")]
    public ColorParameter Colour = new ColorParameter(Color.black);



    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > HDRP Default Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Hidden/Shader/CustomOutlinePP";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume CustomOutlinePP is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        //test
        Vector4 parameters = new Vector4(intensity.value, intensity.value, intensity.value, intensity.value);
        m_Material.SetVector("_Params", parameters);

        m_Material.SetFloat("_Thickness", Thickness.value);
        m_Material.SetFloat("_Edge", Edge.value);
        m_Material.SetFloat("_Transition", Transition.value);
        m_Material.SetColor("_Colour", Colour.value);

        //m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
