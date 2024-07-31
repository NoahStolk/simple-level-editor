#version 330 core

uniform vec4 color;
uniform vec3 cameraPosition;
uniform float fadeMinDistance;
uniform float fadeMaxDistance;
uniform bool fadeOut;

out vec4 FragColor;

in vec4 fragPos;

void main()
{
	vec3 cameraPositionAdjusted = cameraPosition;
	cameraPositionAdjusted.y = fragPos.y;
	float distanceToCamera = length(cameraPositionAdjusted - fragPos.xyz);

	float fadeDistance = fadeMaxDistance - fadeMinDistance;
	FragColor = fadeOut ? color * (1 - clamp((distanceToCamera - fadeMinDistance) / fadeDistance, 0, 1)) : color;
}
