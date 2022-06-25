#version 330 core
out vec4 FragColor;

//���ʽṹ��
struct Material{
	
	sampler2D diffuse;//������ϵ��
	sampler2D specular;//���淴��ǿ��
	float shininess;//����ȣ���32
};

//���սṹ��
struct Light{
	vec3 position;//��Դλ��

	//vec3 direction;//�����

	vec3 ambient;//������ǿ��
	vec3 diffuse;//������ϵ��
	vec3 specular;//�����ǿ��

	//���Դ����
	float constant;
	float linear;
	float quadratic;
};

in vec3 FragPos;//����ռ�����
in vec3 Normal;//������
in vec2 TexCoords;//����

uniform float mixValue;


uniform vec3 viewPos;//���λ��
uniform vec3 lightColor;
//uniform vec3 objectColor;
uniform vec3 lightPos;//��Դλ��

uniform Material material;//���ʶ���
uniform Light light;//��Դ����

void main()
{
	

	//��һ������
	vec3 norm = normalize(Normal);//��������׼��
	//vec3 lightDir = normalize(-light.direction);//������շ���
	vec3 lightDir = normalize(light.position - FragPos);//���Դ���շ���

	//������
	float diff = max(dot(norm,lightDir),0.0);//�����䷽��
	vec3 diffuse = light.diffuse*diff*vec3(texture(material.diffuse,TexCoords));

	//������
	vec3 ambient =  light.ambient*vec3(texture(material.diffuse,TexCoords));//��������

	//���淴��
	vec3 viewDir = normalize(viewPos - FragPos);//���߷���
	vec3 reflectDir = reflect(-lightDir, norm);//�������������߷���ȡ��
	//���㾵�����ǿ��
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);//ȡmaterial.shininess���ݵķ����
	vec3 specular = light.specular*spec * vec3(texture(material.specular,TexCoords));
	
	//���Դ��������
	float distance = length(light.position - FragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	ambient  *= attenuation; 
	diffuse  *= attenuation;
	specular *= attenuation;

	//��ɫ���(������+�������+���淴��)
	vec3 result = ambient+diffuse+specular;
	FragColor = vec4(result,1.0);
}