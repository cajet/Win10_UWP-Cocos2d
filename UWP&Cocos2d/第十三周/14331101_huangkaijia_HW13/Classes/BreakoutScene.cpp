#include "BreakoutScene.h"
#include <string>
using namespace std;
#include "SimpleAudioEngine.h"

using namespace CocosDenshion;

void Breakout::setPhysicsWorld(PhysicsWorld* world) { m_world = world; }

Scene* Breakout::createScene() {
    auto scene = Scene::createWithPhysics();
    /*scene->getPhysicsWorld()->setDebugDrawMask(PhysicsWorld::DEBUGDRAW_ALL);*/
    scene->getPhysicsWorld()->setGravity(Point(0, 0));

    auto layer = Breakout::create();
    layer->setPhysicsWorld(scene->getPhysicsWorld());

    scene->addChild(layer);
    return scene;
}

bool Breakout::init() {
    if (!Layer::init()) {
        return false;
    }

    visibleSize = Director::getInstance()->getVisibleSize();

    preloadMusic();
    //playBgm();

    addBackground();
    addEdge();
    addPlayer();
	addBar();
	addContactListener();
    addTouchListener();
    addKeyboardListener();

    this->schedule(schedule_selector(Breakout::update), 1);
	this->schedule(schedule_selector(Breakout::newBonus), 1);
    return true;
}

void Breakout::preloadMusic() {
    SimpleAudioEngine::getInstance()->preloadBackgroundMusic("music/bgm.mp3");
	SimpleAudioEngine::getInstance()->preloadBackgroundMusic("music/meet_stone.wav");
}

void Breakout::playBgm() {
    SimpleAudioEngine::getInstance()->playBackgroundMusic("music/bgm.mp3", true);
}

void Breakout::addBackground() {
    auto bgsprite = Sprite::create("black_hole_bg0.jpg");
    bgsprite->setPosition(visibleSize / 2);
    bgsprite->setScale(visibleSize.width / bgsprite->getContentSize().width, visibleSize.height / bgsprite->getContentSize().height);
    this->addChild(bgsprite, 0);

    auto ps = ParticleSystemQuad::create("black_hole.plist");
    ps->setPosition(visibleSize / 2);
    this->addChild(ps);
}

void Breakout::addEdge() {
    auto edgeSp = Sprite::create();
    auto boundBody = PhysicsBody::createEdgeBox(visibleSize);
	boundBody->setDynamic(false);
    boundBody->setTag(0);
    edgeSp->setPosition(Point(visibleSize.width / 2, visibleSize.height / 2));
    edgeSp->setPhysicsBody(boundBody);
    this->addChild(edgeSp);
}

void Breakout::addPlayer() {
    player = Sprite::create("player.png");
    player->setAnchorPoint(Vec2(0.5, 0.5));
    player->setPhysicsBody(PhysicsBody::createCircle(35));
	// TODO set bit mask
	player->getPhysicsBody()->setCategoryBitmask(0x02); 
	player->getPhysicsBody()->setContactTestBitmask(0x01);
    player->setPosition(visibleSize / 2);
    player->getPhysicsBody()->setTag(1);
    player->getPhysicsBody()->setAngularVelocityLimit(0);
    addChild(player);
}

void Breakout::addContactListener() {
	auto touchListener = EventListenerPhysicsContact::create();
	touchListener->onContactBegin = CC_CALLBACK_1(Breakout::onConcactBegan, this);
	_eventDispatcher->addEventListenerWithFixedPriority(touchListener, 1);
}

void Breakout::addTouchListener(){
    auto touchListener = EventListenerTouchOneByOne::create();
    touchListener->onTouchBegan = CC_CALLBACK_2(Breakout::onTouchBegan,this);
    touchListener->onTouchMoved = CC_CALLBACK_2(Breakout::onTouchMoved,this);
    touchListener->onTouchEnded = CC_CALLBACK_2(Breakout::onTouchEnded,this);
    _eventDispatcher->addEventListenerWithSceneGraphPriority(touchListener, this);
}

void Breakout::addKeyboardListener() {
    auto keboardListener = EventListenerKeyboard::create();
    keboardListener->onKeyPressed = CC_CALLBACK_2(Breakout::onKeyPressed, this);
    keboardListener->onKeyReleased = CC_CALLBACK_2(Breakout::onKeyReleased, this);
    _eventDispatcher->addEventListenerWithSceneGraphPriority(keboardListener, this);
}

void Breakout::addBar() {
	life = 3;
	time = 30;
	TTFConfig ttfConfig;
	ttfConfig.fontFilePath = "fonts/arial.ttf";
	ttfConfig.fontSize = 30;
	lifebar = Label::createWithTTF(ttfConfig, "Life : 3");
	lifebar->setAnchorPoint(Vec2(0, 0));
	lifebar->setPosition(Vec2(20, visibleSize.height - lifebar->getContentSize().height));
	timebar = Label::createWithTTF(ttfConfig, "Time : 30");
	timebar->setAnchorPoint(Vec2(0, 0));
	timebar->setPosition(Vec2(20, visibleSize.height - lifebar->getContentSize().height * 2));
	addChild(lifebar);
	addChild(timebar);
}

bool Breakout::onTouchBegan(Touch *touch, Event *unused_event) {
	Vec2 postion = touch->getLocation();
	Vec2 dir = postion - player->getPosition();
	dir.normalize();
	player->getPhysicsBody()->setVelocity(dir * 200);
	
	return true;
}

void Breakout::onTouchMoved(Touch *touch, Event *unused_event) {
    Vec2 postion = touch->getLocation();
	Vec2 dir = postion - player->getPosition();
	dir.normalize();
	player->getPhysicsBody()->setVelocity(dir * 200);
	
}

void Breakout::onTouchEnded(Touch *touch, Event *unused_event) {
	player->getPhysicsBody()->setVelocity(Vec2(0, 0));
}

bool Breakout::onConcactBegan(PhysicsContact& contact) {
	auto body1 = contact.getShapeA()->getBody();
	auto body2 = contact.getShapeB()->getBody();
	auto sp1 = (Sprite*)body1->getNode();
	auto sp2 = (Sprite*)body2->getNode();
	auto p = ParticleExplosion::create();
	p->setEmitterMode(ParticleSystem::Mode::RADIUS);
	p->setEndRadius(100);
	if (sp1 && sp2)
	{
		auto tag1 = sp1->getPhysicsBody()->getTag();
		auto tag2 = sp2->getPhysicsBody()->getTag();
		if (tag1 == 10 || tag1 == 20 || tag1 == 30) {
			enemys.eraseObject(body1, false);
			p->setPosition(sp1->getPosition());
			sp1->removeFromParentAndCleanup(true);
			addChild(p);
			char string1[15];
			sprintf(string1, "Life : %d", --life);
			lifebar->setString(string1);
		}
		if (tag2 == 10 || tag2 == 20 || tag2 == 30) {
			enemys.eraseObject(body2, false);
			p->setPosition(sp2->getPosition());
			sp2->removeFromParentAndCleanup(true);
			addChild(p);
			char string1[15];
			sprintf(string1, "Life : %d", --life);
			lifebar->setString(string1);
		}
		if (tag1 == 3) {
			sp1->removeFromParentAndCleanup(true);
			bonus.eraseObject(body1);
			for (auto sp : enemys) {
				auto body = sp->getNode();
				if (body != NULL) {
					ParticleExplosion* explosion = ParticleExplosion::create();
					explosion->setEmitterMode(ParticleSystem::Mode::RADIUS);
					explosion->setEndRadius(100);
					explosion->setPosition(sp->getPosition());
					this->addChild(explosion);
					body->removeFromParentAndCleanup(true);
				}
			}
			enemys.clear();
		}
		if (tag2 == 3) {
			sp2->removeFromParentAndCleanup(true);
			bonus.eraseObject(body2);
			for (auto sp : enemys) {
				auto body = sp->getNode();
				if (body != NULL) {
					ParticleExplosion* explosion = ParticleExplosion::create();
					explosion->setEmitterMode(ParticleSystem::Mode::RADIUS);
					explosion->setEndRadius(100);
					explosion->setPosition(sp->getPosition());
					this->addChild(explosion);
					body->removeFromParentAndCleanup(true);
				}
			}
			enemys.clear();
		}
		SimpleAudioEngine::getInstance()->playEffect("music/meet_stone.wav");
		if (life == 0) gameOver();
	}
	return true;
}

void Breakout::update(float f) {
	newEnemys();
	char string1[15];
	sprintf(string1, "Time : %d", --time);
	timebar->setString(string1);
	if (time == 0) Win();
}

void Breakout::newEnemys() {
	if (enemys.size() > 50) return;
    int newNum = 2;
    while (newNum--) {
        int type = 0;
        if (CCRANDOM_0_1() > 0.85) { type = 2; }
        else if (CCRANDOM_0_1() > 0.6) { type = 1; }

        Point location = Vec2(0, 0);
        switch (rand() % 4)
        {
        case 0:
            location.y = visibleSize.height;
            location.x = rand() % (int)(visibleSize.width);
            break;
        case 1:
            location.x = visibleSize.width;
            location.y = rand() % (int)(visibleSize.height);
            break;
        case 2:
            location.y = 0;
            location.x = rand() % (int)(visibleSize.width);
            break;
        case 3:
            location.x = 0;
            location.y = rand() % (int)(visibleSize.height);
            break;
        default:
            break;
        }
        addEnemy(type, location);
    }
}

void Breakout::addEnemy(int type, Point p) {
    char path[100];
    int tag;
    switch (type)
    {
    case 0:
        sprintf(path, "stone1.png");
        tag = 10;
        break;
    case 1:
        sprintf(path, "stone2.png");
        tag = 20;
        break;
    case 2:
        sprintf(path, "stone3.png");
        tag = 30;
        break;
    default:
        sprintf(path, "stone1.png");
        tag = 10;
        break;
    }
    auto re = Sprite::create(path);
    re->setPhysicsBody(PhysicsBody::createCircle(re->getContentSize().height / 2));
    re->setAnchorPoint(Vec2(0.5, 0.5));
    re->setScale(0.5, 0.5);
    re->setPosition(p);
	// TODO set bitmask
	re->getPhysicsBody()->setCategoryBitmask(0x01);
	re->getPhysicsBody()->setContactTestBitmask(0x02);

    re->getPhysicsBody()->setTag(tag);
    if (rand() % 100 < 50) {
        re->getPhysicsBody()->setVelocity((player->getPosition() - p) * (0.25));
    }
    else {
        re->getPhysicsBody()->setVelocity((Point(rand() % (int)(visibleSize.width - 100) + 50, rand() % (int)(visibleSize.height - 100) + 50) - p) * (0.25));
    }
    re->getPhysicsBody()->setAngularVelocity(CCRANDOM_0_1() * 10);
    enemys.pushBack(re->getPhysicsBody());
    addChild(re);
}

void Breakout::newBonus(float f) {
	if (bonus.size() > 0) return;
	int type = 0;
	if (CCRANDOM_0_1() > 0.85) { type = 2; }
	else if (CCRANDOM_0_1() > 0.6) { type = 1; }
	Point location = Vec2(0, 0);
	switch (rand() % 4)
	{
	case 0:
		location.y = visibleSize.height;
		location.x = rand() % (int)(visibleSize.width);
		break;
	case 1:
		location.x = visibleSize.width;
		location.y = rand() % (int)(visibleSize.height);
		break;
	case 2:
		location.y = 0;
		location.x = rand() % (int)(visibleSize.width);
		break;
	case 3:
		location.x = 0;
		location.y = rand() % (int)(visibleSize.height);
		break;
	default:
		break;
	}

	auto bon = Sprite::create("boom.png");
	bon->setAnchorPoint(Vec2(0.5, 0.5));
	bon->setScale(0.3, 0.3);
	bon->setPhysicsBody(PhysicsBody::createCircle(35));
	bon->setPosition(location);
	bon->getPhysicsBody()->setTag(3);
	bon->getPhysicsBody()->setAngularVelocity(0);
	bon->getPhysicsBody()->setContactTestBitmask(0x02);
	bon->getPhysicsBody()->setCategoryBitmask(0x01);

	if (rand() % 100 < 50) {
		bon->getPhysicsBody()->setVelocity((player->getPosition() - location) * (0.25));
	}
	else {
		bon->getPhysicsBody()->setVelocity((Point(rand() % (int)(visibleSize.width - 100) + 50, rand() % (int)(visibleSize.height - 100) + 50) - location) * (0.25));
	}
	bonus.pushBack(bon->getPhysicsBody());
	this->addChild(bon, 3);
}

void Breakout::onKeyPressed(EventKeyboard::KeyCode code, Event* event) {
    switch (code)
    {
    case cocos2d::EventKeyboard::KeyCode::KEY_LEFT_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_A:
        player->getPhysicsBody()->setVelocity(Point(-200, player->getPhysicsBody()->getVelocity().y));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_D:
        player->getPhysicsBody()->setVelocity(Point(200, player->getPhysicsBody()->getVelocity().y));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_UP_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_W:
        player->getPhysicsBody()->setVelocity(Point(player->getPhysicsBody()->getVelocity().x, 200));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_DOWN_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_S:
        player->getPhysicsBody()->setVelocity(Point(player->getPhysicsBody()->getVelocity().x, -200));
        break;
    default:
        break;
    }
}

void Breakout::onKeyReleased(EventKeyboard::KeyCode code, Event* event) {
    switch (code)
    {
    case cocos2d::EventKeyboard::KeyCode::KEY_LEFT_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_A:
        player->getPhysicsBody()->setVelocity(player->getPhysicsBody()->getVelocity() - Point(-200, 0));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_D:
        player->getPhysicsBody()->setVelocity(player->getPhysicsBody()->getVelocity() - Point(200, 0));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_UP_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_W:
        player->getPhysicsBody()->setVelocity(player->getPhysicsBody()->getVelocity() - Point(0, 200));
        break;
    case cocos2d::EventKeyboard::KeyCode::KEY_DOWN_ARROW:
    case cocos2d::EventKeyboard::KeyCode::KEY_S:
        player->getPhysicsBody()->setVelocity(player->getPhysicsBody()->getVelocity() - Point(0, -200));
        break;
    default:
        break;
    }
}


void Breakout::Win() {
	this->unscheduleAllCallbacks();
	_eventDispatcher->removeAllEventListeners();
	enemys.clear();
	TTFConfig ttfConfig;
	ttfConfig.fontFilePath = "fonts/Marker Felt.ttf";
	ttfConfig.fontSize = 100;
	auto text = Label::createWithTTF(ttfConfig, "YOU WIN");
	text->setAnchorPoint(Vec2(0.5, 0.5));
	text->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	addChild(text, 2);
	Director::getInstance()->pause();
}

void Breakout::gameOver() {
	this->unscheduleAllCallbacks();
	_eventDispatcher->removeAllEventListeners();
	auto p = ParticleExplosion::create();
	p->setPosition(player->getPosition());
	addChild(p);
	player->removeFromParentAndCleanup(true);
	TTFConfig ttfConfig;
	ttfConfig.fontFilePath = "fonts/Marker Felt.ttf";
	ttfConfig.fontSize = 100;
	auto text = Label::createWithTTF(ttfConfig, "GAME OVER");
	text->setAnchorPoint(Vec2(0.5, 0.5));
	text->setPosition(Vec2(visibleSize.width / 2, visibleSize.height / 2));
	addChild(text, 2);
	Director::getInstance()->pause();
}
