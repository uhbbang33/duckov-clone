Shader "Custom/TransparentMask"
{
    Properties
    {
        [IntRange] _StencilRef ("Stencil Reference", Range(0, 255)) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comp", Int) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Int) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"    //1999
        }

        ColorMask 0
        ZWrite Off

        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
            }
        }
    }
}