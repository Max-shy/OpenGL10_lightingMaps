#version 330 core
out vec4 FragColor;

//材质结构体
struct Material{
	
	sampler2D diffuse;//漫反射系数
	sampler2D specular;//镜面反射强度
	float shininess;//反光度，如32
};

//光照结构体
struct Light{
	vec3 position;//光源位置

	//vec3 direction;//定向光

	vec3 ambient;//环境光强度
	vec3 diffuse;//漫反射系数
	vec3 specular;//镜面光强度

	//点光源参数
	float constant;
	float linear;
	float quadratic;
};

in vec3 FragPos;//世界空间坐标
in vec3 Normal;//法向量
in vec2 TexCoords;//纹理

uniform float mixValue;


uniform vec3 viewPos;//相机位置
uniform vec3 lightColor;
//uniform vec3 objectColor;
uniform vec3 lightPos;//光源位置

uniform Material material;//材质对象
uniform Light light;//光源对象

void main()
{
	

	//归一化向量
	vec3 norm = normalize(Normal);//法向量标准化
	//vec3 lightDir = normalize(-light.direction);//单向光照方向
	vec3 lightDir = normalize(light.position - FragPos);//点光源光照方向

	//漫反射
	float diff = max(dot(norm,lightDir),0.0);//漫反射方向
	vec3 diffuse = light.diffuse*diff*vec3(texture(material.diffuse,TexCoords));

	//环境光
	vec3 ambient =  light.ambient*vec3(texture(material.diffuse,TexCoords));//环境反射

	//镜面反射
	vec3 viewDir = normalize(viewPos - FragPos);//视线方向
	vec3 reflectDir = reflect(-lightDir, norm);//反射向量，视线方向取反
	//计算镜面分量强度
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);//取material.shininess次幂的反光度
	vec3 specular = light.specular*spec * vec3(texture(material.specular,TexCoords));
	
	//点光源参数计算
	float distance = length(light.position - FragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	ambient  *= attenuation; 
	diffuse  *= attenuation;
	specular *= attenuation;

	//着色结果(环境光+漫反射光+镜面反射)
	vec3 result = ambient+diffuse+specular;
	FragColor = vec4(result,1.0);
}