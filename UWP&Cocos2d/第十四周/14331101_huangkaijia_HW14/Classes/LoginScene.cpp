#include "LoginScene.h"
#include "cocostudio/CocoStudio.h"
#include "json/rapidjson.h"
#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
#include "Global.h"
#include "GameScene.h"
#include <regex>
#pragma execution_character_set("utf-8")
#define database UserDefault::getInstance()
using std::to_string;
using std::regex;
using std::match_results;
using std::regex_match;
using std::cmatch;
using namespace rapidjson;
USING_NS_CC;

using namespace cocostudio::timeline;

#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
using namespace  rapidjson;

Scene* LoginScene::createScene()
{
    // 'scene' is an autorelease object
    auto scene = Scene::create();
    
    // 'layer' is an autorelease object
    auto layer = LoginScene::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}





// on "init" you need to initialize your instance
bool LoginScene::init()
{
    // 1. super init first
    if ( !Layer::init() )
    {
        return false;
    }
    Size size = Director::getInstance()->getVisibleSize();
    visibleHeight = size.height;
    visibleWidth = size.width;

    textField = TextField::create("Player Name", "Arial", 30);
    textField->setPosition(Size(visibleWidth / 2, visibleHeight / 4 * 3));
    this->addChild(textField, 2);

    auto button = Button::create();
    button->setTitleText("Login");
    button->setTitleFontSize(30);
    button->setPosition(Size(visibleWidth / 2, visibleHeight / 2));
    this->addChild(button, 2);
	button->addClickEventListener(CC_CALLBACK_1(LoginScene::click, this));
    return true;
}

bool LoginScene::click(Ref * teste) {
	Login(textField->getString());
	return true;
}
void LoginScene::Login(string username) {
	HttpRequest* request = new HttpRequest();
	request->setUrl("http://localhost:8080/login");
	request->setRequestType(HttpRequest::Type::POST);
	request->setResponseCallback(CC_CALLBACK_2(LoginScene::onHttpRequestCompleted, this));
	string postData = "username=" + username;
	const char * test = postData.c_str();
	request->setRequestData(postData.c_str(), strlen(test));
	request->setTag("Post Login");
	cocos2d::network::HttpClient::getInstance()->send(request);
	request->release();
}
/*处理登陆后服务端返回的结果的函数*/
void LoginScene::onHttpRequestCompleted(HttpClient * sender, HttpResponse * response) {
	if (!response) {
		return;
	}
	if (!response->isSucceed()) {
		printf("response failed");
		return;
	}
	std::vector<char> * buffer = response->getResponseData();
	std::vector<char> * buffer_ = response->getResponseHeader();
	string str;
	rapidjson::Document d;
	for (int i = 0; i < buffer->size(); i++) {
		str += (*buffer)[i];
	}
	d.Parse<0>(str.c_str());
	if (d.IsObject() && d.HasMember("info")) {
		Global::score = atol(d["info"].GetString());
	}
	string head;
	for (int i = 0; i < buffer_->size(); i++)
		head += (*buffer_)[i];
	Global::gameSessionId =  Global::getSessionIdFromHeader(head);
	database->setStringForKey("key", Global::gameSessionId);
	const char * test = Global::gameSessionId.c_str();
	CCDirector::sharedDirector()->replaceScene(GameScene::createScene());
}