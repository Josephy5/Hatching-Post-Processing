using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//some of the rendering/blit code in Render() is based on IronStar's render feature code in https://github.com/BattleDawnNZ/Image-Effects-with-Shadergraph/blob/master/Assets/Renderer%20Feature/BlitPass.cs
//but the rest is original as most of the code in IronStar's render feature code is unecessary and can be simplified down to this instead.
public class HatchingPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Hatching";

    //material containing the shader
    private Material HatchingMaterial;
    // camera soruces/camera color targets
    private RenderTargetIdentifier source;
    private RenderTargetHandle dest;

    //temporary color texture
    RenderTargetHandle temporaryColorTexture;

    //initializes our variables
    public HatchingPass(RenderPassEvent evt, Material mat)
    {
        renderPassEvent = evt;
        if (mat == null)
        {
            Debug.LogError("No Hatching Material, Please input a material that has the hatching shader into the hatching effect's render feature setting");
            return;
        }

        HatchingMaterial = mat;
        temporaryColorTexture.Init("_TemporaryColorTexture");
    }

    //where our rendering of the effect starts
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (HatchingMaterial == null)
        {
            Debug.LogError("No Hatching Material, Please input a material that has the hatching shader into the hatching effect's render feature setting");
            return;
        }

        //in case if the camera doesn't have the post process option enabled
        //didn't use a debug log cuz it also effects preview camera for any 3d objects/animation and when scene or game is loading.
        //hence, doing so will cause premature logging errors that may not actually be an error
        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }
        //sets command buffer pool/CMD for rendering stuff
        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);

        //releases the CMD for cleanup
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    //sets up the camera color targets to our scripts's private variables of the camera targets
    //x = original camera source, y = destination camera source
    public void Setup(RenderTargetIdentifier x, RenderTargetHandle y)
    {
        source = x; //source
        dest = y; //destination
    }

    //helper method to contain all of our rendering code for the Execute() method
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //modifies the cam data's depth buffer bits to 0
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        //all the blit work/rendering the effect onto the camera color targets/sources
        cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDesc, FilterMode.Point);
        Blit(cmd, source, temporaryColorTexture.Identifier(), HatchingMaterial, 0);
        Blit(cmd, temporaryColorTexture.Identifier(), source);
    }

    //cleans up the temp color texture when is not needed to be used
    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (dest == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
            cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
    }
}
