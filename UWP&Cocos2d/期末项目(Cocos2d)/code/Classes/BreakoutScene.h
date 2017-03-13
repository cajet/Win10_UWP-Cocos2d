#pragma once

#include "cocos2d.h"
#include "SimpleAudioEngine.h"

USING_NS_CC;

class Breakout :public Layer {
public:
	void setPhysicsWorld(PhysicsWorld * world);
	// there's no 'id' in cpp, so we recommend returning the class instance pointer
    static cocos2d::Scene* createScene();

    // Here's a difference. Method 'init' in cocos2d-x returns bool, instead of returning 'id' in cocos2d-iphone
	virtual bool init(PhysicsWorld* world);

    // implement the "static create()" method manually
	static Breakout* create(PhysicsWorld* world);

private:
    //音乐加载与播放
    Sprite* player;
	Sprite* circle;
	Sprite* def;/*防护层*/
	Sprite *clear, *protect, *newLife, *fullblood, *clock;
	Sprite* coin;
	Sprite* boom1, *boom2;
	PhysicsWorld* m_world;
	Label* label1, *label2, *label3, *label4, *label5;
	Size visibleSize;
	cocos2d::Vec2 origin;
	Vector<PhysicsBody*> enemys;
	Vector<PhysicsBody*> rewards;
	int time;
	int holdtime;
	int clr, pro, life, full, clo;
	int HPbarPercentage;
	cocos2d::ProgressTimer* pT;
	cocos2d::Label*timebar;
	cocos2d::Label*lifebar;
	bool flag;
	int savetime;

	void stopAc();

    void preloadMusic();
    void playBgm();
	void playMeet();

    void addBackground();
    void addEdge();
	void addPlayer();

	void addContactListener();
	void addTouchListener();
	void addKeyboardListener();
	void removeFromEnemys(int tag, bool flag2);
	void addRewardLabel(int tag);
	void Duang(int tag);

	void update(float f);
	void update2(float f);
	bool onConcactBegan(PhysicsContact& contact);

    bool onTouchBegan(Touch *touch, Event *unused_event);
    void onTouchMoved(Touch *touch, Event *unused_event);
    void onTouchEnded(Touch *touch, Event *unused_event);
    void onKeyPressed(EventKeyboard::KeyCode code, Event* event);
    void onKeyReleased(EventKeyboard::KeyCode code, Event* event);

	void newEnemys();
	void newReward();
	void set_size();
	void addJoint();
	void addEnemy(int type, Point p);
	void addReward(int type, Point p);
	void gameOver();
	void returnTOmenue(cocos2d::Ref* pSender);
	void restart(cocos2d::Ref* pSender);
	void submit_time(cocos2d::Ref* pSender);
};