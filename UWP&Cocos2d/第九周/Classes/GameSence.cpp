#include "GameSence.h"

USING_NS_CC;

Scene* GameSence::createScene()
{
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = GameSence::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

// on "init" you need to initialize your instance
bool GameSence::init()
{

	if (!Layer::init())
	{
		return false;
	}

	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	/*设置背景*/
	auto bg0 = Sprite::create("level-background-0.jpg");
	bg0->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	this->addChild(bg0, 0);

	/*shoot按钮*/
	auto shootitem = MenuItemLabel::create(Label::createWithTTF("shoot", "fonts/Marker Felt.ttf", 70),
		CC_CALLBACK_1(GameSence::shootMenuCallback, this));
	shootitem->ignoreAnchorPointForPosition(true);
	shootitem->setPosition(Point(visibleSize.width / 2 + 200, 450));
	auto menu = Menu::create(shootitem, NULL);
	menu->setPosition(Vec2::ZERO);
	this->addChild(menu, 1);

	/*设置石头层*/
	stoneLayer = Layer::create();
	stoneLayer->setAnchorPoint(Point(0, 0));
	stoneLayer->setPosition(Point(0, 0));

	stone = Sprite::create("stone.png");
	stone->setPosition(Point(540, 480));
	stoneLayer->addChild(stone);

	/*设置老鼠层*/
	mouseLayer = Layer::create();
	mouseLayer->setAnchorPoint(Point(0, 0));
	mouseLayer->setPosition(Point(0, visibleSize.height / 2));

	mouse = Sprite::createWithSpriteFrameName("gem-mouse-0.png");
	Animate* mouseAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("mouseAnimation"));
	mouse->runAction(RepeatForever::create(mouseAnimate));
	mouse->setPosition(Point(visibleSize.height / 2, 0));
	mouseLayer->addChild(mouse);

	addChild(stoneLayer);
	addChild(mouseLayer);

	//add touch listener
	EventListenerTouchOneByOne* listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(GameSence::onTouchBegan, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, mouseLayer);

	return true;
}

bool GameSence::onTouchBegan(Touch *touch, Event *unused_event) {
	auto location = touch->getLocation();
	// 获取当前点击点所在相对按钮的位置坐标，本地坐标与世界坐标的转化
	Point p = Director::getInstance()->convertToGL(touch->getLocation());
	p = mouseLayer->convertToNodeSpace(p);
	p = Point(p.x, -p.y);
	/*添加奶酪，并实现消失动作*/
	auto chess = Sprite::create("cheese.png");
	chess->setPosition(Point(location.x, location.y));
	this->addChild(chess, 0);
	auto fadeout = FadeOut::create(4.0f);
	chess->runAction(fadeout);
	/*老鼠跑向奶酪动作*/
	auto move = MoveTo::create(2.0f, p);
	mouse->runAction(move);
	auto turn = RotateBy::create(1.0f, 180.0f);
	mouse->runAction(turn);
	return true;
}

void GameSence::shootMenuCallback(cocos2d::Ref* pSender) {
	/*获取老鼠当前位置location，因为这里懒得再去用本地坐标与世界坐标的转化，所以直接把y+300 (●'◡'●)*/
	auto x = mouse->getPosition().x;
	auto y= mouse->getPosition().y;
	auto location = Point(x, y + 300);
	/*老鼠放下钻石,并避开石头*/
	auto diam = Sprite::create("diamond.png");
	diam->setPosition(Point(location.x, location.y));
	this->addChild(diam, 0);
	Point p2 = Point(100, 100);
	auto move2 = MoveTo::create(1.5f, p2);
	mouse->runAction(move2);
	/*石头飞向老鼠当前位置*/
	auto move = MoveTo::create(1.2f, location);
	stone->runAction(move);
	auto fadeout = FadeOut::create(2.5f);
	stone->runAction(fadeout);
	/*发射石头后再新建石头*/
	stone = Sprite::create("stone.png");
	stone->setPosition(Point(540, 480));
	stoneLayer->addChild(stone);

}