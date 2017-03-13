#include"Monster.h"
USING_NS_CC;

Factory* Factory::factory = NULL;
Factory::Factory() {
	initSpriteFrame();
}
Factory* Factory::getInstance() {
	if (factory == NULL) {
		factory = new Factory();
	}
	return factory;
}
//π÷ŒÔÀ¿Õˆ÷°∂Øª≠
void Factory::initSpriteFrame(){
	auto texture = Director::getInstance()->getTextureCache()->addImage("Monster.png");
	monsterDead.reserve(7);
	for (int i = 0; i < 7; i++) {
		auto frame = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(258-48*i,0,42,42)));
		monsterDead.pushBack(frame);
	}
}

Sprite* Factory::createMonster() {
	Sprite* mons = Sprite::create("Monster.png", CC_RECT_PIXELS_TO_POINTS(Rect(364,0,42,42)));
	monster.pushBack(mons);
	return mons;
}
/*“∆≥˝π÷ŒÔ*/
void Factory::removeMonster(Sprite* sp) {
	cocos2d:Vector<Sprite*>::iterator it = monster.begin();
	for (; it != monster.end();) {
		if ((*it) == sp) {
			/*÷¥––π÷ŒÔÀ¿Õˆ∂Øª≠*/
			auto animation = Animation::createWithSpriteFrames(monsterDead, 1.0f);
			auto animate = Animate::create(animation);
			auto fadeout = FadeOut::create(1.5f);
			auto myspawn = Spawn::createWithTwoActions(animate, fadeout);
			(*it)->runAction(myspawn);
			monster.erase(it);
			break;
		}
		else {
			it++;
		}
	}
}
/*π÷ŒÔ“∆∂Ø*/
void Factory::moveMonster(Vec2 playerPos,float time){
	cocos2d::Vector<Sprite*>::iterator it = monster.begin();
	for (; it != monster.end(); it++) {
		Vec2 monsterPos = (*it)->getPosition();
		Vec2 direction = playerPos - monsterPos;
		direction.normalize();
		(*it)->runAction(MoveBy::create(time, direction * 30));
	}
}
//≈–∂œ≈ˆ◊≤
Sprite* Factory::collider(Rect rect) {
	cocos2d::Vector<Sprite*>::iterator it = monster.begin();
	for (; it != monster.end(); it++) {
		Rect monsterRect = (*it)->getBoundingBox();
		if (rect.intersectsRect(monsterRect)) {
			return (*it);
		}
	}
}