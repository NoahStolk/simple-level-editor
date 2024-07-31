#version 330 core

uniform vec4 color;
uniform vec3 cameraPosition;
uniform float maxDistance;
uniform bool fadeOut;

out vec4 FragColor;

in vec4 fragPos;

void main()
{
	float distanceToCamera = length(cameraPosition - fragPos.xyz);

	const float minDistance = 25;
	float fadeDistance = maxDistance - minDistance;
	FragColor = fadeOut ? color * (1 - clamp((distanceToCamera - minDistance) / fadeDistance, 0, 1)) : color;
}
