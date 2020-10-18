datablock ParticleData(CoinParticle : DefaultParticle) {
	lifetimeMS = 1000;
	gravityCoefficient = 0;
	dragCoefficient = 2;
	
	sizes[0] = 1;
	sizes[1] = 1;
	sizes[2] = 1;
	sizes[3] = 1;
	inheritedVelFactor = 0;
};
datablock ParticleEmitterData(CoinEmitter : DefaultEmitter) {
	particles = CoinParticle;
	
	ejectionPeriodMS = 10;
	ejectionVelocity = 4.167;
	ejectionOffset = 0.625;
	thetaMax = 180;
	softnessDistance = 1;
	lifetimeMS = 200;
};
datablock ParticleEmitterNodeData(CoinNode : DefaultEmitterNodeData) {
	timeMultiple = 1.0;
};