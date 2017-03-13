#include "BreakoutScene3.h"
#include "MenuSence.h"
#include "SimpleAudioEngine.h"
#include <string>

using namespace std;
using namespace CocosDenshion;
USING_NS_CC;

void Breakout3::setPhysicsWorld(PhysicsWorld* world) { m_world = world; }

Scene* Breakout3::createScene() {
	auto scene = Scene::createWithPhysics();
	//scene->getPhysicsWorld()->setDebugDrawMask(PhysicsWorld::DEBUGDRAW_ALL);
	scene->getPhysicsWorld()->setGravity(Point(0, 0));

	auto layer = Breakout3::create(scene->getPhysicsWorld());
	scene->addChild(layer);
	return scene;
}

bool Breakout3::init(PhysicsWorld* world) {
	if (!Layer::init()) {
		return false;
	}

	this->setPhysicsWorld(world);
	visibleSize = Director::getInstance()->getVisibleSize();
	origin = Director::getInstance()->getVisibleOrigin();
	//初始化状态变量
	holdtime = 0;
	clr = 0, pro = 0, life = 0, full = 0, clo = 0;
	flag = false;

	//增加血条
	Sprite* sp0 = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(0, 320, 420, 47)));
	Sprite* sp = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(610, 362, 4, 16)));
	pT = ProgressTimer::create(sp);
	pT->setScaleX(90);
	pT->setAnchorPoint(Vec2(0, 0));
	pT->setType(ProgressTimerType::BAR);
	pT->setBarChangeRate(Point(1, 0));
	pT->setMidpoint(Point(0, 1));

	HPbarPercentage = 100;
	pT->setPercentage(HPbarPercentage);
	pT->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width + 10, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(pT, 1);
	sp0->setAnchorPoint(Vec2(0, 0));
	sp0->setPosition(Vec2(origin.x + pT->getContentSize().width + 10, origin.y + visibleSize.height - sp0->getContentSize().height));
	addChild(sp0, 0);

	TTFConfig ttfConfig;
	ttfConfig.fontFilePath = "fonts/arial.ttf";
	ttfConfig.fontSize = 30;
	timebar = Label::createWithTTF(ttfConfig, "Time : 0");
	timebar->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width + 450, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(timebar, 1);
	lifebar = Label::createWithTTF(ttfConfig, "100");
	lifebar->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width - 20, origin.y + visibleSize.height - 2 * pT->getContentSize().height + 5));
	addChild(lifebar, 1);

	preloadMusic();
	playBgm();

	addBackground();
	addEdge();
	addPlayer();

	addContactListener();
	addTouchListener();
	addKeyboardListener();

	this->schedule(schedule_selector(Breakout3::update), 1);
	return true;
}

Breakout3 * Breakout3::create(PhysicsWorld * world) {
	Breakout3* pRet = new(std::nothrow) Breakout3();
	if (pRet && pRet->init(world)) {
		pRet->autorelease();
		return pRet;
	}
	delete pRet;
	pRet = NULL;
	return NULL;
}

void Breakout3::preloadMusic() {
	SimpleAudioEngine::getInstance()->preloadBackgroundMusic("music/bgm.mp3");
	SimpleAudioEngine::getInstance()->preloadEffect("music/meet_stone.wav");
	SimpleAudioEngine::getInstance()->preloadEffect("music/meet_reward.wav");
}

void Breakout3::playBgm() {
	SimpleAudioEngine::getInstance()->playBackgroundMusic("music/bgm.mp3", true);
}

void Breakout3::playMeet() {
	SimpleAudioEngine::getInstance()->playEffect("music/meet_stone.wav", false, 1.0f, 1.0f, 1.0f);
}

void Breakout3::addBackground() {
	auto bgsprite = Sprite::create("black_hole_bg2.jpg");
	bgsprite->setPosition(visibleSize / 2);
	bgsprite->setScale(Director::getInstance()->getContentScaleFactor());
	this->addChild(bgsprite, 0);

	auto ps = ParticleSystemQuad::create("black_hole.plist");
	ps->setPosition(visibleSize / 2);
	this->addChild(ps);
}

void Breakout3::addEdge() {
	auto edgeSp = Sprite::create();
	auto boundBody = PhysicsBody::createEdgeBox(visibleSize);
	boundBody->setDynamic(false);
	boundBody->setTag(0);
	edgeSp->setPosition(Point(visibleSize.width / 2, visibleSize.height / 2));
	edgeSp->setScale(Director::getInstance()->getContentScaleFactor());
	edgeSp->setPhysicsBody(boundBody);
	this->addChild(edgeSp);
}

//添加玩家和左下角的特效
void Breakout3::addPlayer() {
	player = Sprite::create("player.png");
	player->setAnchorPoint(Vec2(0.5, 0.5));
	player->setPhysicsBody(PhysicsBody::createCircle(player->getContentSize().height / 2));
	player->getPhysicsBody()->setCategoryBitmask(0x01);
	player->getPhysicsBody()->setCollisionBitmask(0x03);
	player->getPhysicsBody()->setContactTestBitmask(0xFFFFFFFF);
	player->getPhysicsBody()->setTag(1);
	player->setPosition(visibleSize / 2);
	player->getPhysicsBody()->setAngularVelocityLimit(0);
	addChild(player);

	def = Sprite::create("protect.png");
	def->setVisible(false);
	addChild(def);

	clear = Sprite::create("clear.PNG");
	clear->setPosition(clear->getContentSize().width + origin.x, clear->getContentSize().height / 2 + origin.y);
	clear->setScale(0.8, 0.8);

	protect = Sprite::create("protect.PNG");
	protect->setPosition(clear->getPositionX() + protect->getContentSize().width, clear->getPositionY());
	protect->setScale(0.8, 0.8);

	newLife = Sprite::create("newLife.PNG");
	newLife->setPosition(protect->getPositionX() + newLife->getContentSize().width - 10, protect->getPositionY());
	newLife->setScale(0.6, 0.6);

	fullblood = Sprite::create("fullblood.PNG");
	fullblood->setPosition(newLife->getPositionX() + fullblood->getContentSize().width, newLife->getPositionY());
	fullblood->setScale(0.8, 0.8);

	clock = Sprite::create("clock.PNG");
	clock->setPosition(fullblood->getPositionX() + clock->getContentSize().width, fullblood->getPositionY());
	clock->setScale(0.8, 0.8);

	addChild(clear, 3);
	addChild(protect, 3);
	addChild(newLife, 3);
	addChild(fullblood, 3);
	addChild(clock, 3);

	label1 = Label::createWithTTF("0", "fonts/arial.ttf", 20);
	label1->setPosition(clear->getPositionX() + 18, clear->getPositionY() + 18);
	label1->setColor(ccc3(248, 248, 255));
	addChild(label1, 4);
	label2 = Label::createWithTTF("0", "fonts/arial.ttf", 20);
	label2->setPosition(protect->getPositionX() + 18, protect->getPositionY() + 18);
	label2->setColor(ccc3(248, 248, 255));
	addChild(label2, 4);
	label3 = Label::createWithTTF("0", "fonts/arial.ttf", 20);
	label3->setPosition(newLife->getPositionX() + 18, newLife->getPositionY() + 18);
	label3->setColor(ccc3(248, 248, 255));
	addChild(label3, 4);
	label4 = Label::createWithTTF("0", "fonts/arial.ttf", 20);
	label4->setPosition(fullblood->getPositionX() + 18, fullblood->getPositionY() + 18);
	label4->setColor(ccc3(248, 248, 255));
	addChild(label4, 4);
	label5 = Label::createWithTTF("0", "fonts/arial.ttf", 20);
	label5->setPosition(clock->getPositionX() + 18, clock->getPositionY() + 18);
	label5->setColor(ccc3(248, 248, 255));
	addChild(label5, 4);
}

void Breakout3::addContactListener() {
	auto touchListener = EventListenerPhysicsContact::create();
	touchListener->onContactBegin = CC_CALLBACK_1(Breakout3::onConcactBegan, this);
	_eventDispatcher->addEventListenerWithFixedPriority(touchListener, 1);
}

void Breakout3::addTouchListener() {
	auto touchListener = EventListenerTouchOneByOne::create();
	touchListener->onTouchBegan = CC_CALLBACK_2(Breakout3::onTouchBegan, this);
	touchListener->onTouchMoved = CC_CALLBACK_2(Breakout3::onTouchMoved, this);
	touchListener->onTouchEnded = CC_CALLBACK_2(Breakout3::onTouchEnded, this);
	_eventDispatcher->addEventListenerWithSceneGraphPriority(touchListener, this);
}

//1、2、3依次对应不同的特效
void Breakout3::addKeyboardListener() {
	auto keboardListener = EventListenerKeyboard::create();
	keboardListener->onKeyPressed = CC_CALLBACK_2(Breakout3::onKeyPressed, this);
	keboardListener->onKeyReleased = CC_CALLBACK_2(Breakout3::onKeyReleased, this);
	_eventDispatcher->addEventListenerWithSceneGraphPriority(keboardListener, this);
}

bool Breakout3::onTouchBegan(Touch *touch, Event *unused_event) {
	Vec2 postion = touch->getLocation();
	auto vec = postion - player->getPosition();
	vec = vec.getNormalized();
	player->getPhysicsBody()->setVelocity((vec) * (60));
	return true;
}

void Breakout3::onTouchMoved(Touch *touch, Event *unused_event) {
}

void Breakout3::onTouchEnded(Touch *touch, Event *unused_event) {
}

//特效标签 得到奖励加1 使用一次减1
void Breakout3::addRewardLabel(int tag) {
	SimpleAudioEngine::getInstance()->playEffect("music/meet_reward.wav", false, 1.0f, 1.0f, 1.0f);

	std::stringstream s;
	std::string number;
	Vec2 pos;
	switch (tag)
	{
	case 40:
		clr++;
		s << clr;
		s >> number;
		label1->setString(number);
		break;
	case 50:
		pro++;
		s << pro;
		s >> number;
		label2->setString(number);
		break;
	case 60:
		life++;
		s << life;
		s >> number;
		label3->setString(number);
		break;
	case 70:
		full++;
		s << full;
		s >> number;
		label4->setString(number);
	case 80:
		clo++;
		s << clo;
		s >> number;
		label5->setString(number);
	default:
		break;
	}

	Vector<PhysicsBody*>::iterator it = rewards.begin();
	for (; it != rewards.end(); it++) {
		if ((*it)->getTag() == tag) {
			rewards.erase(it);
			return;
		}
	}
}

//移除stone并播放效果
void Breakout3::removeFromEnemys(int tag, bool flag2) {
	playMeet();
	/*flag2为false时，即开启防护罩，此时不减血*/
	if (flag2) {
		/*根据陨石大小减血*/
		if (tag == 10) HPbarPercentage -= 8;
		else if (tag == 20) HPbarPercentage -= 10;
		else if (tag == 30) HPbarPercentage -= 20;
		else if (tag == 15) HPbarPercentage -= 30;
		if (HPbarPercentage <= 0) {
			HPbarPercentage = 0;
			char string1[15];
			sprintf(string1, "%d", HPbarPercentage);
			lifebar->setString(string1);
			stopAc();
		}
		char string2[15];
		sprintf(string2, "%d", HPbarPercentage);
		lifebar->setString(string2);
		pT->setPercentage(HPbarPercentage);
	}
	Vec2 pos;
	for (auto it = enemys.begin(); it != enemys.end(); it++) {
		if ((*it) != NULL && (*it)->getTag() == tag) {
			pos = (*it)->getPosition();
			enemys.erase(it);
			break;
		}
	}
	pos = player->getPosition();
	auto explosion = ParticleSystemQuad::create("explode.plist");
	explosion->setPosition(pos);
	addChild(explosion);
}

bool Breakout3::onConcactBegan(PhysicsContact& contact) {
	auto nodeA = contact.getShapeA()->getBody();
	auto nodeB = contact.getShapeB()->getBody();
	if (nodeA && nodeB) {
		if (nodeA->getTag() == 1) {
			if (nodeB->getTag() > 30) { /*大于30的tag是技能*/
				if (nodeB->getTag() != 90) {
					addRewardLabel(nodeB->getTag());
					nodeB->getNode()->removeFromParentAndCleanup(true);
				}
				else { /*为90即命运硬币*/
					nodeB->getNode()->removeFromParentAndCleanup(true);
					coin = NULL;
					set_size();
				}
			}
			else {
				if (!flag) removeFromEnemys(nodeB->getTag(), true); /*没有防护罩*/
				else removeFromEnemys(nodeB->getTag(), false);
				nodeB->getNode()->removeFromParentAndCleanup(true);
			}
		}
		else if (nodeB->getTag() == 1) {
			if (nodeA->getTag() > 30) {
				if (nodeA->getTag() != 90) {
					addRewardLabel(nodeA->getTag());
					nodeA->getNode()->removeFromParentAndCleanup(true);
				}
				else {
					nodeA->getNode()->removeFromParentAndCleanup(true);
					coin = NULL;
					set_size();
				}
			}
			else {
				if (!flag) removeFromEnemys(nodeA->getTag(), true);
				else removeFromEnemys(nodeA->getTag(), false);
				nodeA->getNode()->removeFromParentAndCleanup(true);
			}
		}
		return true;
	}
	return false;
}

void Breakout3::stopAc() {
	this->unscheduleAllCallbacks();
	_eventDispatcher->removeAllEventListeners();
	SimpleAudioEngine::getInstance()->pauseBackgroundMusic();
	gameOver();
}

void Breakout3::update(float f) {
	holdtime++;
	char string1[15];
	sprintf(string1, "Time : %d", holdtime);
	timebar->setString(string1);
	if (HPbarPercentage <= 0) {
		stopAc();
	}
	/*4秒的防护罩时间*/
	if (holdtime - savetime > 4) {
		savetime = -1;
		flag = false;
	}
	/*显示防护层*/
	if (flag == true) {
		def->setVisible(true);
		this->schedule(schedule_selector(Breakout3::update2), 0.01);
	}
	else {
		def->setVisible(false);
	}

	newEnemys();
	newReward();
	/*每隔10秒出现命运硬币*/
	if (holdtime % 10 == 0) {
		if (coin == NULL) {
			coin = Sprite::create("coin.png");
			coin->setPosition(RandomHelper::random_int(100, 450), RandomHelper::random_int(100, 450));
			coin->setScale(0.3);
			coin->setAnchorPoint(Vec2(0.5, 0.5));
			coin->setPhysicsBody(PhysicsBody::createCircle(45));
			coin->getPhysicsBody()->setAngularVelocityLimit(0);
			coin->getPhysicsBody()->setCategoryBitmask(0x02);
			coin->getPhysicsBody()->setCollisionBitmask(0x01);
			coin->getPhysicsBody()->setContactTestBitmask(0xFFFFFFFF);
			coin->getPhysicsBody()->setTag(90);
			this->addChild(coin);
		}
	}
}
/*每0.01秒调用一次获取飞机位置，使防护罩实时跟进飞机*/
void Breakout3::update2(float f) {
	Vec2 pos;
	pos = player->getPosition();
	def->setPosition(pos);
}

//奖励随机出现
void Breakout3::newReward() {
	if (holdtime % 5 == 0) {
		if (rewards.size() > 5) return;
		/*设置各技能奖励出现的概率*/
		int type = 4;/*防护罩*/
		if (CCRANDOM_0_1() > 0.95) { type = 6; } /*满血*/
		else if (CCRANDOM_0_1() > 0.85) { type = 3; } /*全屏爆炸*/
		else if (CCRANDOM_0_1() > 0.80) { type = 7; }/*时间增加5s*/
		else if (CCRANDOM_0_1() > 0.75) { type = 5; }/*血加20%*/
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
		addReward(type, location);
	}
}

void Breakout3::addReward(int type, Point p) {
	char path[100];
	int tag;
	switch (type)
	{
	case 3:
		sprintf(path, "clear.PNG");
		tag = 40;
		break;
	case 4:
		sprintf(path, "protect.PNG");
		tag = 50;
		break;
	case 5:
		sprintf(path, "newLife.PNG");
		tag = 60;
		break;
	case 6:
		sprintf(path, "fullblood.PNG");
		tag = 70;
		break;
	case 7:
		sprintf(path, "clock.PNG");
		tag = 80;
		break;
	default:
		break;
	}
	auto re = Sprite::create(path);
	re->setPhysicsBody(PhysicsBody::createCircle(re->getContentSize().height / 2));
	re->setAnchorPoint(Vec2(0.5, 0.5));
	re->setScale(0.7, 0.7);
	re->setPosition(p);
	re->getPhysicsBody()->setCategoryBitmask(0x02);
	re->getPhysicsBody()->setCollisionBitmask(0x01);
	re->getPhysicsBody()->setContactTestBitmask(0xFFFFFFFF);
	re->getPhysicsBody()->setTag(tag);
	if (rand() % 100 < 50) {
		re->getPhysicsBody()->setVelocity((player->getPosition() - p) * (0.25));
	}
	else {
		re->getPhysicsBody()->setVelocity((Point(rand() % (int)(visibleSize.width - 100) + 50, rand() % (int)(visibleSize.height - 100) + 50) - p) * (0.25));
	}
	re->getPhysicsBody()->setAngularVelocity(CCRANDOM_0_1() * 2);
	rewards.pushBack(re->getPhysicsBody());
	addChild(re);
}

void Breakout3::newEnemys() {
	if (enemys.size() > 50) return;
	int newNum = 2;
	while (newNum--) {
		int type = 0;
		if (CCRANDOM_0_1() > 0.92) { type = 11; } /*最大号陨石*/
		else if (CCRANDOM_0_1() > 0.85) { type = 2; }
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

void Breakout3::addEnemy(int type, Point p) {
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
	case 11:
		sprintf(path, "stone4.png");
		tag = 15;
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
	re->getPhysicsBody()->setCategoryBitmask(0x02);
	re->getPhysicsBody()->setCollisionBitmask(0x01);
	re->getPhysicsBody()->setContactTestBitmask(0xFFFFFFFF);
	// TODO set bitmask
	re->getPhysicsBody()->setTag(tag);
	if (rand() % 100 < 30) {
		re->getPhysicsBody()->setVelocity((player->getPosition() - p) * (0.25));
	}
	else {
		re->getPhysicsBody()->setVelocity((Point(rand() % (int)(visibleSize.width - 100) + 50, rand() % (int)(visibleSize.height - 100) + 50) - p) * (0.25));
	}
	re->getPhysicsBody()->setAngularVelocity(CCRANDOM_0_1() * 8);
	enemys.pushBack(re->getPhysicsBody());
	addChild(re);
}

void Breakout3::Duang(int tag) {
	switch (tag)
	{
	case 40:
		if (clr > 0) {
			Vec2 pos;
			for (auto it = enemys.begin(); it != enemys.end(); ) {
				if ((*it) != NULL && (*it)->getNode() != NULL) {
					pos = (*it)->getPosition();
					auto explosion = ParticleSystemQuad::create("explode.plist");
					explosion->setPosition(pos);
					addChild(explosion);
					playMeet();
					(*it)->getNode()->removeFromParentAndCleanup(true);
				}
				it = enemys.erase(it);
			}
			clr -= 2;
			addRewardLabel(40);
		}
		break;
	case 50:
		savetime = holdtime;
		flag = true;
		pro -= 2;
		addRewardLabel(50);
		break;
	case 60:
		HPbarPercentage += 20;
		if (HPbarPercentage >= 100) HPbarPercentage = 100;
		pT->setPercentage(HPbarPercentage);
		char string1[15];
		sprintf(string1, "%d", HPbarPercentage);
		lifebar->setString(string1);
		life -= 2;
		addRewardLabel(60);
		break;
	case 70:
		HPbarPercentage = 100;
		pT->setPercentage(HPbarPercentage);
		char string2[15];
		sprintf(string2, "%d", HPbarPercentage);
		lifebar->setString(string2);
		full -= 2;
		addRewardLabel(70);
		break;
	case 80:
		holdtime += 5;
		clo -= 2;
		addRewardLabel(80);
		break;
	default:
		break;
	}
}
/*命运硬币，可使player随机变大或变小一倍，概率都是50%*/
void Breakout3::set_size() {
	double temp = CCRANDOM_0_1();
	if (temp >= 0.5) {
		auto scale = ScaleTo::create(0, 0.5);
		auto scale2 = ScaleTo::create(0, 1);
		auto squence = Sequence::create(scale, DelayTime::create(5), scale2, nullptr);
		player->runAction(squence);
	}
	else {
		auto scale = ScaleTo::create(0, 2);
		auto scale2 = ScaleTo::create(0, 1);
		auto squence = Sequence::create(scale, DelayTime::create(5), scale2, nullptr);
		player->runAction(squence);
	}
}

void Breakout3::onKeyPressed(EventKeyboard::KeyCode code, Event* event) {
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
	case cocos2d::EventKeyboard::KeyCode::KEY_1:
		if (clr > 0) Duang(40);
		break;
	case cocos2d::EventKeyboard::KeyCode::KEY_2:
		if (pro > 0) Duang(50);
		break;
	case cocos2d::EventKeyboard::KeyCode::KEY_3:
		if (life > 0) Duang(60);
		break;
	case cocos2d::EventKeyboard::KeyCode::KEY_4:
		if (full > 0) Duang(70);
		break;
	case cocos2d::EventKeyboard::KeyCode::KEY_5:
		if (clo > 0) Duang(80);
		break;
	default:
		break;
	}
}

void Breakout3::onKeyReleased(EventKeyboard::KeyCode code, Event* event) {
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

void Breakout3::gameOver() {
	cocos2d::Label* label;
	label = Label::createWithTTF("Game Over", "fonts/old evils.ttf", 100);
	label->setPosition(Vec2(origin.x + visibleSize.width / 2, origin.y + visibleSize.height / 2 + 40));
	this->addChild(label, 2);
	// 返回主菜单游戏按钮
	auto returnitem = MenuItemLabel::create(Label::createWithTTF("Return to Menue", "fonts/ALTEA.ttf", 20),
		CC_CALLBACK_1(Breakout3::returnTOmenue, this));
	returnitem->ignoreAnchorPointForPosition(true);
	returnitem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 160, origin.y + visibleSize.height / 2 - 80));
	auto menu = Menu::create(returnitem, NULL);
	menu->setPosition(Vec2::ZERO);
	this->addChild(menu, 1);
	//提交到服务器
	auto submititem = MenuItemLabel::create(Label::createWithTTF("Submit", "fonts/ALTEA.ttf", 20),
		CC_CALLBACK_1(Breakout3::submit_time, this));
	submititem->ignoreAnchorPointForPosition(true);
	submititem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 60, origin.y + visibleSize.height / 2 - 130));
	auto menu2 = Menu::create(submititem, NULL);
	menu2->setPosition(Vec2::ZERO);
	this->addChild(menu2, 1);
	//重新开始
	auto restartitem = MenuItemLabel::create(Label::createWithTTF("Restart", "fonts/ALTEA.ttf", 20),
		CC_CALLBACK_1(Breakout3::restart, this));
	restartitem->ignoreAnchorPointForPosition(true);
	restartitem->setPosition(Vec2(origin.x + visibleSize.width / 2 - 60, origin.y + visibleSize.height / 2 - 180));
	auto menu3 = Menu::create(restartitem, NULL);
	menu3->setPosition(Vec2::ZERO);
	this->addChild(menu3, 1);

}

void Breakout3::returnTOmenue(cocos2d::Ref* pSender)
{
	auto scene = MenuSence::createScene();
	Director::getInstance()->replaceScene(
	TransitionFade::create(0.5, scene, Color3B(0, 255, 255)));
}

void Breakout3::submit_time(cocos2d::Ref* pSender)
{

}

void Breakout3::restart(cocos2d::Ref* pSender) 
{
	auto scene = Breakout3::createScene();
	Director::getInstance()->replaceScene(
	TransitionFade::create(0.5, scene, Color3B(0, 255, 255)));
}
