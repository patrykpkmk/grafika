#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

float4x4 WorldInverseTranspose;

float3 DirectionalLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1.0;
float3 ViewVector = float3(1, 0, 0);

float4 ModelColor = float4(1,1,1,1);

float3 SpotlightOneLightPosition = float3(0,20,0);
float3 SpotlightOneSpotDirection = float3(0,-1,0);
float SpotlightOneLightRadius = 5.0f;
float SpotlightOneSpotDecayExponent = 5.0f;
float SpotlightOneSpotLightAngleCosine = 0.2f;
float4 SpotlightOneDiffuseColor = float4(1, 1, 1, 1); 
float4 SpotlightOneSpecularColor = float4(1, 1, 1, 1); 

float3 SpotlightTwoLightPosition = float3(0,20,0);
float3 SpotlightTwoSpotDirection = float3(0,-1,0);
float SpotlightTwoLightRadius = 5.0f;
float SpotlightTwoSpotDecayExponent = 5.0f;
float SpotlightTwoSpotLightAngleCosine = 0.2f;
float4 SpotlightTwoDiffuseColor = float4(1, 1, 1, 1); 
float4 SpotlightTwoSpecularColor = float4(1, 1, 1, 1); 

texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;  	
    float3 Normal : NORMAL0; 
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0; 
    float3 Normal : NORMAL0;
	float4 OriginalPosition : POSITION1;
};

struct VertexShaderInputTx
{
    float4 Position : POSITION0;  	
    float3 Normal : NORMAL0; 
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutputTx
{
    float4 Position : POSITION0;
    float4 Color : COLOR0; 
    float3 Normal : NORMAL0;
	float4 OriginalPosition : POSITION1;
	float2 TextureCoordinate : TEXCOORD0;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	output.OriginalPosition = input.Position;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Normal = normal;
    output.Color = ModelColor; 
		
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{   
	float4 firstSpotLightColor = max(AmbientColor * AmbientIntensity * input.Color,0);
	
	float3 normal = normalize(input.Normal);
	
    float3 lightVector = normalize(SpotlightOneLightPosition - input.OriginalPosition);
	float attenuation = saturate(1.0f - length(lightVector)/SpotlightOneLightRadius);
	SpotlightOneSpotDirection = normalize(SpotlightOneSpotDirection);
	float SdL = dot(SpotlightOneSpotDirection,-lightVector);	
	if (SdL > SpotlightOneSpotLightAngleCosine) 
	{
		float spotIntensity = pow(SdL,SpotlightOneSpotDecayExponent);
		float lightIntensity = dot(normal, lightVector);
		float4 diffuse = SpotlightOneDiffuseColor * DiffuseIntensity * lightIntensity;
	
		float3 r = normalize(2 * dot(normal, lightVector) * normal - lightVector);
		float3 v = normalize(mul(normalize(ViewVector), World));
		float dotProduct = -dot(r, v);
		float4 specular = SpecularIntensity * SpotlightOneSpecularColor * max(pow(dotProduct, Shininess), 0);
		
		firstSpotLightColor = saturate(firstSpotLightColor + attenuation * spotIntensity * (max(diffuse * input.Color, 0) + max(0,specular)) );
	}
	float4 secondSpotLightColor = firstSpotLightColor;
	float3 lightVector2 = normalize(SpotlightTwoLightPosition - input.OriginalPosition);
	float attenuation2 = saturate(1.0f - length(lightVector2)/SpotlightTwoLightRadius);
	SpotlightTwoSpotDirection = normalize(SpotlightTwoSpotDirection);
	float SdL2 = dot(SpotlightTwoSpotDirection,-lightVector2);	
	if (SdL2 > SpotlightTwoSpotLightAngleCosine) 
	{
		float spotIntensity2 = pow(SdL2,SpotlightTwoSpotDecayExponent);
		float lightIntensity2 = dot(normal, lightVector2);
		float4 diffuse2 = SpotlightTwoDiffuseColor * DiffuseIntensity * lightIntensity2;
	
		float3 r2 = normalize(2 * dot(normal, lightVector2) * normal - lightVector2);
		float3 v2 = normalize(mul(normalize(ViewVector), World));
		float dotProduct2 = -dot(r2, v2);
		float4 specular2 = SpecularIntensity * SpotlightTwoSpecularColor * max(pow(dotProduct2, Shininess), 0) ;

		secondSpotLightColor = secondSpotLightColor + saturate(attenuation2 * spotIntensity2 * (max(diffuse2 * input.Color, 0) + max(0,specular2)) );
	}
	
	float4 directionalLightColor = secondSpotLightColor;
	float lightIntensity3 = dot(normal, DirectionalLightDirection);
    float4 diffuse3 = DiffuseColor * DiffuseIntensity * lightIntensity3;
	
    float3 light = normalize(DirectionalLightDirection);
    float3 r3 = normalize(2 * dot(normal, light) * normal - light);
    float3 v3 = normalize(mul(normalize(ViewVector), World));
    float dotProduct3 = -dot(r3, v3);
    float4 specular3 = SpecularIntensity * SpecularColor * max(pow(dotProduct3, Shininess), 0) ;

    return saturate(directionalLightColor + max(diffuse3 * input.Color,0)+ max(specular3,0));
}

//TEXTURES

VertexShaderOutputTx VertexShaderFunctionTx(VertexShaderInputTx input)
{
    VertexShaderOutputTx output;
	output.OriginalPosition = input.Position;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Normal = normal;
    output.Color = ModelColor; 
	
	output.TextureCoordinate = input.TextureCoordinate;
	
    return output;
}

float4 PixelShaderFunctionTx(VertexShaderOutputTx input) : COLOR0
{   
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1;

	float4 firstSpotLightColor = max(AmbientColor * AmbientIntensity * input.Color,0) * textureColor;
	
	float3 normal = normalize(input.Normal);
	
    float3 lightVector = normalize(SpotlightOneLightPosition - input.OriginalPosition);
	float attenuation = saturate(1.0f - length(lightVector)/SpotlightOneLightRadius);
	SpotlightOneSpotDirection = normalize(SpotlightOneSpotDirection);
	float SdL = dot(SpotlightOneSpotDirection,-lightVector);	
	if (SdL > SpotlightOneSpotLightAngleCosine) 
	{
		float spotIntensity = pow(SdL,SpotlightOneSpotDecayExponent);
		float lightIntensity = dot(normal, lightVector);
		float4 diffuse = SpotlightOneDiffuseColor * DiffuseIntensity * lightIntensity;
	
		float3 r = normalize(2 * dot(normal, lightVector) * normal - lightVector);
		float3 v = normalize(mul(normalize(ViewVector), World));
		float dotProduct = -dot(r, v);
		float4 specular = SpecularIntensity * SpotlightOneSpecularColor * max(pow(dotProduct, Shininess), 0) ;
		
		firstSpotLightColor = saturate(firstSpotLightColor + attenuation * spotIntensity * (max(diffuse * input.Color, 0) + max(0,specular)) );
	}
	float4 secondSpotLightColor = firstSpotLightColor;
	float3 lightVector2 = normalize(SpotlightTwoLightPosition - input.OriginalPosition);
	float attenuation2 = saturate(1.0f - length(lightVector2)/SpotlightTwoLightRadius);
	SpotlightTwoSpotDirection = normalize(SpotlightTwoSpotDirection);
	float SdL2 = dot(SpotlightTwoSpotDirection,-lightVector2);	
	if (SdL2 > SpotlightTwoSpotLightAngleCosine) 
	{
		float spotIntensity2 = pow(SdL2,SpotlightTwoSpotDecayExponent);
		float lightIntensity2 = dot(normal, lightVector2);
		float4 diffuse2 = SpotlightTwoDiffuseColor * DiffuseIntensity * lightIntensity2;
	
		float3 r2 = normalize(2 * dot(normal, lightVector2) * normal - lightVector2);
		float3 v2 = normalize(mul(normalize(ViewVector), World));
		float dotProduct2 = -dot(r2, v2);
		float4 specular2 = SpecularIntensity * SpotlightTwoSpecularColor * max(pow(dotProduct2, Shininess), 0) ;

		secondSpotLightColor = secondSpotLightColor + saturate(attenuation2 * spotIntensity2 * (max(diffuse2 * input.Color, 0) + max(0,specular2)) );
	}
	
	float4 directionalLightColor = secondSpotLightColor;
	float lightIntensity3 = dot(normal, DirectionalLightDirection);
    float4 diffuse3 = DiffuseColor * DiffuseIntensity * lightIntensity3;
	
    float3 light = normalize(DirectionalLightDirection);
    float3 r3 = normalize(2 * dot(normal, light) * normal - light);
    float3 v3 = normalize(mul(normalize(ViewVector), World));
    float dotProduct3 = -dot(r3, v3);
    float4 specular3 = SpecularIntensity * SpecularColor * max(pow(dotProduct3, Shininess), 0) ;
	
    return saturate(directionalLightColor + max(diffuse3 * input.Color,0) * textureColor+ max(specular3,0));
}

technique NotTextured
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}

technique Textured
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunctionTx();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunctionTx();
    }
}