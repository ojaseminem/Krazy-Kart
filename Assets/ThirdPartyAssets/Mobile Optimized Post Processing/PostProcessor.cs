using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Main post processing class where image effects are applied to scene texture
/// </summary>
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessor : MonoBehaviour {
    
	/// <summary>
    /// Class holding information about shaders uniforms names
    /// </summary>
	private static class ShaderUniforms {
        
		//Vignette shader
		public static readonly int VignettePower = Shader.PropertyToID("_VignettePower");
        public static readonly int VignetteCenter = Shader.PropertyToID("_VignetteCenter");

		//Blur shader
		public static readonly int BlurDir = Shader.PropertyToID("_Dir");

		//ToneMapping shader
		public static readonly int Gamma = Shader.PropertyToID("_Gamma");
		public static readonly int Exposure = Shader.PropertyToID("_Exposure");

		//LUT shader
		public static readonly int LUT = Shader.PropertyToID("_LUT");
		public static readonly int LUTIntensity = Shader.PropertyToID("_LUTIntensity");
		public static readonly int ColorsAmount = Shader.PropertyToID("_ColorsAmount");

		//Chromatic abberation shader
		public static readonly int ColorsShiftAmount = Shader.PropertyToID("_ColorShiftFactor");
		public static readonly int FishEyeEffectFactor = Shader.PropertyToID("_FishEyeEffectFactor");
		public static readonly int FishEyeStart = Shader.PropertyToID("_FishEyeEffectStart");
		public static readonly int FishEyeEnd = Shader.PropertyToID("_FishEyeEffectEnd");

		//Bloom combine shader
		public static readonly int BloomFilteredTexture = Shader.PropertyToID("_FilteredTex");
		
		//Bloom filter shader
		public static readonly int BloomValuesCombined = Shader.PropertyToID("_ValuesCombined");
		public static readonly int BloomIntensity = Shader.PropertyToID("_Intensity");
	}
	

	[Tooltip("Post processor settings, to create new press right click in project view then: Create -> Mobile Optimized Post Processing -> PostProcessorSettings")]
	public PostProcessorSettings Settings;

	/// <summary>
	/// Dictionary containing material instances used by post processor
	/// </summary>
	/// <typeparam name="string">Path to shader file for example "Hidden/VignetteShader"</typeparam>
	/// <typeparam name="Material">Material instance created from shader find by path (key)</typeparam>
	/// <returns></returns>
	private Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    
	private const string VignetteShader = "Hidden/VignetteShader";	
	private const string BlurShader = "Hidden/BlurShader";
	private const string LUTShader = "Hidden/LUTShader";	
	private const string ToneMappingShader = "Hidden/ToneMappingShader";
	private const string ChromaticAbberationShader = "Hidden/ChromaticAbberationShader";

	private const string BloomFilterShader = "Hidden/BloomFilterShader";
	private const string BloomCombineShader = "Hidden/BloomCombineShader";

	/// <summary>
	/// Inform about lack of settings user only once to avoid spamming same warning over and over again in console
	/// </summary>
	private bool InformedAboutLackOfSettings = false;

	/// <summary>
	/// Method for obtaining material instance by supplying shader file path
	/// </summary>
	/// <param name="shaderName">Shader file path</param>
	/// <returns></returns>
	public Material GetMaterial(string shaderName) {
		Material material;
		if(Materials.TryGetValue(shaderName, out material)) {
			return material;
		} else {
			Shader shader = Shader.Find(shaderName);

			if(shader == null) {
				Debug.LogError("Shader not found (" + shaderName  + "), check if missed shader is in Shaders folder if not reimport this package. If this problem occurs only in build try to add all shaders in Shaders folder to Always Included Shaders (Project Settings -> Graphics -> Always Included Shaders)");
			}
			
			Material NewMaterial = new Material(shader);
			NewMaterial.hideFlags = HideFlags.HideAndDontSave;	
			Materials.Add(shaderName, NewMaterial);
			return NewMaterial;
		}
	}

	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture) {
		RenderTexture currentTarget = sourceTexture;

		//if settings are null just passthrough source image
		if(Settings == null) {
			Graphics.Blit(currentTarget, destTexture);
			
			//check if allready informed about lack of post processing settings to avoid spamming with same message in log console
			if(!InformedAboutLackOfSettings) {
				Debug.LogWarning("Please attach post processor settings!");
				InformedAboutLackOfSettings = true;
			}

			return;
		}
		InformedAboutLackOfSettings = false;

		//Apply bloom effect
		if(Settings.BloomEnabled) {
			RenderTexture bloomTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			ApplyBloom(currentTarget, bloomTarget, sourceTexture);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = bloomTarget;
		}

		//Apply color grading effects
		if(Settings.ColorGradingEnabled) {
			currentTarget = ApplyColorGrading(currentTarget, sourceTexture);
		}

		//Apply Vignette effect
		if(Settings.VignetteEnabled) {
			RenderTexture vignetteTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			ApplyVignette(currentTarget, vignetteTarget);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = vignetteTarget;
		}

		//Apply blur whole screen effect
		if(Settings.BlurEnabled && Settings.BlurRadius > 0) {
			RenderTexture Blurred = ApplyBlur(currentTarget, sourceTexture, Settings.BlurRadius, Settings.BlurTextureResolutionDivider, Settings.BlurIterations);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = Blurred;
		}

		//Copy last render target to screen buffer
		Graphics.Blit(currentTarget, destTexture);
		ReleaseTemporary(currentTarget, sourceTexture);
	}

	/// <summary>
	/// Helper function to avoid releasing sourceTexture by a mistake
	/// </summary>
	/// <param name="texture">Texture you want to release</param>
	/// <param name="sourceTexture">Original source texture</param>	
	private void ReleaseTemporary(RenderTexture texture, RenderTexture sourceTexture) {
		if(texture != sourceTexture) {
			RenderTexture.ReleaseTemporary(texture);
		}
	}

	private void ApplyVignette(RenderTexture from, RenderTexture to) {
		Material VignetteMaterial = GetMaterial(VignetteShader);
		VignetteMaterial.SetFloat(ShaderUniforms.VignettePower, Settings.VignettePower);
        VignetteMaterial.SetVector(ShaderUniforms.VignetteCenter, Settings.VignetteCenter);
		Graphics.Blit(from, to, VignetteMaterial);
	}

	private RenderTexture ApplyBlur(RenderTexture currentTarget, RenderTexture sourceTexture, float BlurRadius, float BlurTextureResolutionDivider, int Iterations) {
		Material BlurMaterial = GetMaterial(BlurShader);
		Vector2 BlurDirection1 = new Vector2(BlurRadius, 0);
		Vector2 BlurDirection2 = new Vector2(0, BlurRadius);

		BlurMaterial.SetVector(ShaderUniforms.BlurDir, BlurDirection1);
		RenderTexture blurTarget = RenderTexture.GetTemporary((int) (currentTarget.width / BlurTextureResolutionDivider), 
															(int) (currentTarget.height / BlurTextureResolutionDivider));
		FilterMode mode = FilterMode.Bilinear;
		
		blurTarget.filterMode = mode;

		//first iteration
		Graphics.Blit(currentTarget, blurTarget, BlurMaterial);
			
		for(int i = 1; i < Iterations; i++) {
			RenderTexture nextTarget = RenderTexture.GetTemporary((int) (currentTarget.width / BlurTextureResolutionDivider), 
																(int) (currentTarget.height / BlurTextureResolutionDivider));
				
			BlurMaterial.SetVector(ShaderUniforms.BlurDir, (i % 2 == 0) ? BlurDirection2 : BlurDirection1);

			Graphics.Blit(blurTarget, nextTarget, BlurMaterial);
			ReleaseTemporary(blurTarget, sourceTexture);
			blurTarget = nextTarget;
			
			blurTarget.filterMode = mode;
		}

		return blurTarget;
	}

	private RenderTexture ApplyColorGrading(RenderTexture currentTarget, RenderTexture sourceTexture) {
		if(Settings.ToneMappingEnabled) {
			RenderTexture ToneMappingTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			Material ToneMappingMaterial = GetMaterial(ToneMappingShader);
			ToneMappingMaterial.SetFloat(ShaderUniforms.Gamma, Settings.Gamma);
			ToneMappingMaterial.SetFloat(ShaderUniforms.Exposure, Settings.Exposure);
				
			Graphics.Blit(currentTarget, ToneMappingTarget, ToneMappingMaterial);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = ToneMappingTarget;
		}

		if(Settings.LUTEnabled) {
			RenderTexture LUTTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			Material LUTMaterial = GetMaterial(LUTShader);
			LUTMaterial.SetFloat(ShaderUniforms.LUTIntensity, Settings.LUTIntensity);
			LUTMaterial.SetFloat(ShaderUniforms.ColorsAmount, Settings.ColorsAmount);
			LUTMaterial.SetTexture(ShaderUniforms.LUT, Settings.LUT);

			Graphics.Blit(currentTarget, LUTTarget, LUTMaterial);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = LUTTarget;
		}

		if(Settings.ChromaticAbberationEnabled) {
			RenderTexture CHBTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			Material CHBMaterial = GetMaterial(ChromaticAbberationShader);
			CHBMaterial.SetFloat(ShaderUniforms.ColorsShiftAmount, Settings.ColorsShiftAmount);
			CHBMaterial.SetFloat(ShaderUniforms.FishEyeEffectFactor, Settings.FishEyeEffectFactor);

			CHBMaterial.SetFloat(ShaderUniforms.FishEyeStart, Settings.FishEyeEffectStart);
			CHBMaterial.SetFloat(ShaderUniforms.FishEyeEnd, Settings.FishEyeEffectEnd);

			Graphics.Blit(currentTarget, CHBTarget, CHBMaterial);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = CHBTarget;
		}

		return currentTarget;
	}

	private void ApplyBloom(RenderTexture from, RenderTexture to, RenderTexture sourceTexture) {
		//first filter glowing parts of scene
		RenderTexture FilteredScene = RenderTexture.GetTemporary((int) (from.width / Settings.FilterTextureResolutionDivider), (int) (from.height / Settings.FilterTextureResolutionDivider));
		FilteredScene.filterMode = FilterMode.Trilinear;

		Material FilterMaterial = GetMaterial(BloomFilterShader);

		//precompute bloom filter paramters there to save gpu performance
		float knee = Settings.BloomThreshold * Settings.BloomSoftThreshold;
		Vector4 filter = new Vector4();
		filter.x = Settings.BloomThreshold;
		filter.y = filter.x - knee;
		filter.z = 2f * knee;
		filter.w = 0.25f / (knee + 0.00001f);
		FilterMaterial.SetVector(ShaderUniforms.BloomValuesCombined, filter);
		FilterMaterial.SetFloat(ShaderUniforms.BloomIntensity, Mathf.GammaToLinearSpace(Settings.BloomIntensity));
		Graphics.Blit(from, FilteredScene, FilterMaterial);

		//blur filtered scene image
		RenderTexture BlurredFilteredScene = ApplyBlur(FilteredScene, sourceTexture, Settings.BloomBlurRadius, Settings.BloomBlurTextureResolutionDivider, Settings.BloomBlurIterations);
		ReleaseTemporary(FilteredScene, sourceTexture);

		if(Settings.BloomDebugView) {
			Graphics.Blit(BlurredFilteredScene, to);
			ReleaseTemporary(BlurredFilteredScene, sourceTexture);
		} else {
			Material CombineMaterial = GetMaterial(BloomCombineShader);
			CombineMaterial.SetTexture(ShaderUniforms.BloomFilteredTexture, BlurredFilteredScene);
			Graphics.Blit(from, to, CombineMaterial);
			ReleaseTemporary(BlurredFilteredScene, sourceTexture);
		}
	}

	/// <summary>
	/// Destroy all material instances created for post processing 
	/// </summary>
	void OnDisable() {
		foreach(string shader in Materials.Keys) {
			Material material;
			if(Materials.TryGetValue(shader, out material)) {
				DestroyImmediate(material);
			}
		}
		Materials.Clear();
	}
}
