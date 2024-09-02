using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HatchingRenderFeature : ScriptableRendererFeature
{
    //initialzing the render feature settings
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        //the material that contains the hatching effect's shader, user must put in manually for now
        public Material material;
    }
    public Settings settings = new Settings();

    HatchingPass m_HatchingPass;

    //sets the hatching's render pass up
    public override void Create()
    {
        this.name = "Hatching Pass";
        if (settings.material == null)
        {
            Debug.LogWarning("No Hatching Material, Please input a material that has the hatching shader into the hatching effect's render feature setting");
            return;
        }
        m_HatchingPass = new HatchingPass(settings.renderPassEvent, settings.material);
    }
    //call and adds the hatching render pass to the scriptable renderer's queue
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //camera color targets for the render pass
        var src = renderer.cameraColorTarget;
        var dest = RenderTargetHandle.CameraTarget;

        if (settings.material == null)
        {
            Debug.LogWarning("No Hatching Material, Please input a material that has the hatching shader into the hatching effect's render feature setting");
            return;
        }
        //passes the camera color targets to the render pass script
        m_HatchingPass.Setup(src, dest);
        renderer.EnqueuePass(m_HatchingPass);
    }
}
