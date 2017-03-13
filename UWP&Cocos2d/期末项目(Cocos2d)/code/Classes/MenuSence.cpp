#include "MenuSence.h"
#include "BreakoutScene.h"
#include "BreakoutScene2.h"
#include "BreakoutScene3.h"
#pragma execution_character_set("utf-8")
USING_NS_CC;

Scene* MenuSence::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = MenuSence::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

// on "init" you need to initialize your instance
bool MenuSence::init()
{

    if ( !Layer::init() )
    {
        return false;
    }
    
    Size visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();
	
	auto startitem = MenuItemLabel::create(Label::createWithTTF("简单版", "fonts/坊哥特体简体中文.ttf", 40),
		CC_CALLBACK_1(MenuSence::start1, this));
	startitem->ignoreAnchorPointForPosition(true);
	startitem->setPosition(Vec2(origin.x + visibleSize.width / 2-50 , origin.y + visibleSize.height / 2 ));
	auto menu1 = Menu::create(startitem, NULL);
	menu1->setPosition(Vec2::ZERO);
	this->addChild(menu1, 1);

	auto startitem2 = MenuItemLabel::create(Label::createWithTTF("中级版", "fonts/坊哥特体简体中文.ttf", 40),
		CC_CALLBACK_1(MenuSence::start2, this));
	startitem2->ignoreAnchorPointForPosition(true);
	startitem2->setPosition(Vec2(origin.x + visibleSize.width / 2 - 50, origin.y + visibleSize.height / 2 - 60));
	auto menu2 = Menu::create(startitem2, NULL);
	menu2->setPosition(Vec2::ZERO);
	this->addChild(menu2, 1);

	auto startitem3 = MenuItemLabel::create(Label::createWithTTF("究极版", "fonts/坊哥特体简体中文.ttf", 40),
		CC_CALLBACK_1(MenuSence::start3, this));
	startitem3->ignoreAnchorPointForPosition(true);
	startitem3->setPosition(Vec2(origin.x + visibleSize.width / 2 - 50, origin.y + visibleSize.height / 2-120));
	auto menu3 = Menu::create(startitem3, NULL);
	menu3->setPosition(Vec2::ZERO);
	this->addChild(menu3, 1);

	auto bgsprite = Sprite::create("menubg.jpg");
	bgsprite->setPosition(visibleSize / 2);
	bgsprite->setScale(Director::getInstance()->getContentScaleFactor());
	this->addChild(bgsprite, 0);

	auto label2 = Label::createWithTTF("是XX就坚持100s", "fonts/坊哥特体简体中文.ttf", 60);
	label2->setPosition(Vec2(origin.x + visibleSize.width / 2 - 10, origin.y + visibleSize.height / 2 +160));
	label2->setColor(ccc3(255,215,0));
	addChild(label2, 0);

    return true;
}

void MenuSence::start1(Ref* pSender) {
	/*添加转场特效*/
	auto mysense = Breakout2::createScene();
	auto replacesense= CCTransitionJumpZoom::create(2, mysense);
	Director::sharedDirector()->replaceScene(replacesense);
}


void MenuSence::start2(Ref* pSender) {
	auto mysense = Breakout3::createScene();
	auto replacesense = CCTransitionJumpZoom::create(2, mysense);
	Director::sharedDirector()->replaceScene(replacesense);
}

void MenuSence::start3(Ref* pSender) {
	auto mysense = Breakout::createScene();
	auto replacesense = CCTransitionJumpZoom::create(2, mysense);
	Director::sharedDirector()->replaceScene(replacesense);
}

