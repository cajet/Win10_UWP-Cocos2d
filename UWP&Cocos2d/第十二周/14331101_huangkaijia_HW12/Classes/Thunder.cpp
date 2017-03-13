#include "Thunder.h"
#include "cocostudio/CocoStudio.h"
#include "ui/CocosGUI.h"
#include <algorithm>

USING_NS_CC;

using namespace CocosDenshion;

Scene* Thunder::createScene() {
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = Thunder::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

bool Thunder::init() {
    if ( !Layer::init() ) {
        return false;
    }

    visibleSize = Director::getInstance()->getVisibleSize();

    auto bgsprite = Sprite::create("background.jpg");
    bgsprite->setPosition(visibleSize / 2);
    // bgsprite->setScale(visibleSize.width / bgsprite->getContentSize().width, \
    //     visibleSize.height / bgsprite->getContentSize().height);
    this->addChild(bgsprite, 0);

    player = Sprite::create("player.png");
    player->setPosition(visibleSize.width / 2, player->getContentSize().height + 5);
    player->setName("player");
    this->addChild(player, 1);

    addEnemy(5);

    preloadMusic();
    playBgm();

    addTouchListener();
    addKeyboardListener();
    addCustomListener();

    // TODO
    // add schedule
	schedule(schedule_selector(Thunder::update));
    return true;
}

void Thunder::preloadMusic() {
	auto audio = SimpleAudioEngine::getInstance();
	audio->preloadBackgroundMusic("music/bgm.mp3");
	audio->preloadEffect("music/fire.wav");
	audio->preloadEffect("music/explore.wav");
}

void Thunder::playBgm() {
	auto audio = SimpleAudioEngine::getInstance();
	audio->playBackgroundMusic("music/bgm.mp3", true);
}

void Thunder::addEnemy(int n) {
    enemys.resize(n * 3);
    for(unsigned i = 0; i < 3; ++i) {
        char enemyPath[20];
        sprintf(enemyPath, "stone%d.png", 3 - i);
        double width  = (visibleSize.width - 20) / (n + 1.0),
               height = visibleSize.height - (50 * (i + 1));
        for(int j = 0; j < n; ++j) {
            auto enemy = Sprite::create(enemyPath);
            enemy->setAnchorPoint(Vec2(0.5, 0.5));
            enemy->setScale(0.5, 0.5);
            enemy->setPosition(width * (j + 1), height);
            enemys[i * n + j] = enemy;
			addChild(enemy);
        }
    }
}

void Thunder::addTouchListener(){
	auto touchListener = EventListenerTouchOneByOne::create();
	touchListener->onTouchBegan = CC_CALLBACK_2(Thunder::onTouchBegan, this);
	touchListener->onTouchMoved = CC_CALLBACK_2(Thunder::onTouchMoved, this);
	touchListener->onTouchEnded = CC_CALLBACK_2(Thunder::onTouchEnded, this);
	_eventDispatcher->addEventListenerWithSceneGraphPriority(touchListener, this);
}

void Thunder::addKeyboardListener() {
    auto keyboardlistener= EventListenerKeyboard::create();
	keyboardlistener->onKeyPressed = CC_CALLBACK_2(Thunder::onKeyPressed, this);
	keyboardlistener->onKeyReleased = CC_CALLBACK_2(Thunder::onKeyReleased, this);
	_eventDispatcher->addEventListenerWithSceneGraphPriority(keyboardlistener, player);
}

void Thunder::update(float f) {
	//移动飞船，并保持在屏幕可视区内
	if (move == -5 && player->getPositionX() > 30) {
		auto moveby = MoveBy::create(f, Vec2(-5, 0));
		player->runAction(moveby);
	}
	else if (move == 5 && player->getPositionX() < visibleSize.width - 20) {
		auto moveby = MoveBy::create(f, Vec2(5, 0));
		player->runAction(moveby);
	}

	static double count = 0;
	static int dir = 1;
	count += f;
	//移动子弹并判断是否移出边缘
	for (std::vector<Sprite*>::iterator it = bullet.begin(); it != bullet.end(); it++) {
		auto moveby = MoveBy::create(f, Vec2(0, 5));
		(*it)->runAction(moveby);
		if ((*it)->getPositionY() > visibleSize.height - 10) {
			(*it)->removeFromParentAndCleanup(true);
			bullet.erase(it);
			break;
		}
	}

	for (unsigned i = 0; i < enemys.size(); i++) {
		if (enemys[i] != NULL) {
			//移动陨石
			auto moveby1 = MoveBy::create(f, Vec2(dir, 0));
			enemys[i]->runAction(moveby1);
			if (count > 1) {
				auto moveby2 = MoveBy::create(f, Vec2(0, -10));
				enemys[i]->runAction(moveby2);
			}
			//检测子弹与陨石碰撞
			for (std::vector<Sprite*>::iterator it = bullet.begin(); it != bullet.end(); it++) {
				if ((*it)->getPosition().getDistance(enemys[i]->getPosition()) < 30) {
					EventCustom e("meet");
					e.setUserData(&i);
					_eventDispatcher->dispatchEvent(&e);
					(*it)->removeFromParentAndCleanup(true);
					it = bullet.erase(it);
					break;
				}
			}
			//判断游戏是否失败，并显示You Lose
			if (enemys[i] != NULL && enemys[i]->getPositionY() < player->getContentSize().height * 1.5 + 5) {
				player->setVisible(false);
				SimpleAudioEngine::getInstance()->playEffect("music/explore.wav", false);
				auto label = Label::createWithTTF("You Lose!!!", "fonts/Marker Felt.ttf", 100);
				label->setPosition(Vec2(visibleSize.width / 2,
					visibleSize.height / 2 - label->getContentSize().height+150));
				this->addChild(label, 1);
				Director::getInstance()->pause();
			}
		}
	}
	if (count > 1) { count = 0.0; dir = -dir; }
}

void Thunder::fire() {
	auto newbullet = Sprite::create("bullet.png");
	newbullet->setPosition(player->getPosition());
	bullet.push_back(newbullet);
	addChild(newbullet,1);
	SimpleAudioEngine::getInstance()->playEffect("music/fire.wav", false);
}

void Thunder::addCustomListener() {
	auto meetListener =
		EventListenerCustom::create("meet", CC_CALLBACK_1(Thunder::meet, this));
	_eventDispatcher->addEventListenerWithFixedPriority(meetListener, 1);
}

bool Thunder::onTouchBegan(Touch *touch, Event *unused_event) {
	/*根据屏幕宽度一半判断是左移还是右移*/
	if (touch->getLocation().x < visibleSize.width / 2) move = -5;
	else move = 5;
	return true;
}

void Thunder::onTouchMoved(Touch *touch, Event *unused_event) {

}

void Thunder::onTouchEnded(Touch *touch, Event *unused_event) {
	move = 0;
}

void Thunder::onKeyPressed(EventKeyboard::KeyCode code, Event* event) {
    switch (code) {
        case cocos2d::EventKeyboard::KeyCode::KEY_LEFT_ARROW:
        case cocos2d::EventKeyboard::KeyCode::KEY_A:
			move = -5;
            break;
        case cocos2d::EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
        case cocos2d::EventKeyboard::KeyCode::KEY_D:
			move = 5;
            break;
        case cocos2d::EventKeyboard::KeyCode::KEY_SPACE:
            fire();
            break;
        default:
            break;
    }
}

void Thunder::onKeyReleased(EventKeyboard::KeyCode code, Event* event) {
	switch (code) {
	case cocos2d::EventKeyboard::KeyCode::KEY_LEFT_ARROW:
	case cocos2d::EventKeyboard::KeyCode::KEY_A:
		move = 0;
		break;
	case cocos2d::EventKeyboard::KeyCode::KEY_RIGHT_ARROW:
	case cocos2d::EventKeyboard::KeyCode::KEY_D:
		move = 0;
		break;
	default:
		break;
	}
}

void Thunder::meet(EventCustom* event) {
	int* temp = static_cast<int*>(event->getUserData());
	enemys[*temp]->removeFromParentAndCleanup(true);
	enemys[*temp] = NULL;
	SimpleAudioEngine::getInstance()->playEffect("music/explore.wav", false);
}

