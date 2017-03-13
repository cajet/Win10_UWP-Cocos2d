#include "GameScene.h"
#include "json/rapidjson.h"
#include "json/document.h"
#include "json/writer.h"
#include "json/stringbuffer.h"
#include <regex>
#include <map>
using std::regex;
using std::match_results;
using std::regex_match;
using std::cmatch;
using namespace rapidjson;

USING_NS_CC;

cocos2d::Scene* GameScene::createScene() {
    // 'scene' is an autorelease object
    auto scene = Scene::create();

    // 'layer' is an autorelease object
    auto layer = GameScene::create();

    // add layer as a child to scene
    scene->addChild(layer);

    // return the scene
    return scene;
}

bool GameScene::init() {
    if (!Layer::init())
    {
        return false;
    }

    Size size = Director::getInstance()->getVisibleSize();
    visibleHeight = size.height;
    visibleWidth = size.width;

    score_field = TextField::create("Score", "Arial", 30);
    score_field->setPosition(Size(visibleWidth / 4, visibleHeight / 4 * 3));
	char a[100];
	sprintf(a, "%ld", Global::score);
	score_field->setString(a);
    this->addChild(score_field, 2);

    submit_button = Button::create();
    submit_button->setTitleText("Submit");
    submit_button->setTitleFontSize(30);
    submit_button->setPosition(Size(visibleWidth / 4, visibleHeight / 4));
	submit_button->addClickEventListener(CC_CALLBACK_1(GameScene::submit, this));
    this->addChild(submit_button, 2);

    rank_field = TextField::create("", "Arial", 30);
    rank_field->setPosition(Size(visibleWidth / 4 * 3, visibleHeight / 4 * 3));
    this->addChild(rank_field, 2);

    rank_button = Button::create();
    rank_button->setTitleText("Rank");
    rank_button->setTitleFontSize(30);
	rank_button->addClickEventListener(CC_CALLBACK_1(GameScene::rank, this));
    rank_button->setPosition(Size(visibleWidth / 4 * 3, visibleHeight / 4));
    this->addChild(rank_button, 2);
	/*未对score赋值时初始化为0*/
	if (Global::score == 0) {
		submit(NULL);
	}

    return true;
}


bool GameScene::submit(Ref * test) {
	HttpRequest* request = new HttpRequest();
	request->setUrl("http://localhost:8080/submit");
	request->setRequestType(HttpRequest::Type::POST);
	request->setResponseCallback(CC_CALLBACK_2(GameScene::onSubmitCompleted, this));
	string postData = "score=" + score_field->getString();
	request->setRequestData(postData.c_str(), postData.size());
	request->setTag("Post Submit");
	vector<string> header;
	/*本地化保存GAMESESSIONID等cookies信息*/
	string test1 = "Cookie: GAMESESSIONID=" + Global::gameSessionId;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);

	cocos2d::network::HttpClient::getInstance()->send(request);
	request->release();
	return true;
}

bool GameScene::rank(Ref * test) {
	HttpRequest* request = new HttpRequest();
	request->setUrl("http://localhost:8080/rank?top=10");
	request->setRequestType(HttpRequest::Type::GET);
	request->setResponseCallback(CC_CALLBACK_2(GameScene::onRankCompleted, this));
	request->setTag("Get rank");
	vector<string> header;
	/*本地化保存GAMESESSIONID等cookies信息*/
	string test1 = "Cookie: GAMESESSIONID=" + Global::gameSessionId;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);

	cocos2d::network::HttpClient::getInstance()->send(request);
	request->release();
	return true;
}
/*处理submit后服务器返回结果的函数*/
void GameScene::onSubmitCompleted(HttpClient * sender, HttpResponse * response) {
	std::vector<char> * buffer = response->getResponseData();
	string str;
	rapidjson::Document d;
	for (int i = 0; i < buffer->size(); i++) {
		str += (*buffer)[i];
	}
	d.Parse<0>(str.c_str());
	if (d.IsObject() && d.HasMember("info")) {
		Global::score = atol(d["info"].GetString());
		char a[100];
		sprintf(a, "%ld", Global::score);
		score_field->setString(a);
	}
}
/*处理rank后服务器返回结果的函数*/
void GameScene::onRankCompleted(HttpClient * sender, HttpResponse * response) {
	if (!response) {
		return;
	}
	if (!response->isSucceed()) {
		log("respone failed");
		return;
	}
	vector<char> *_header = response->getResponseHeader();
	vector<char> *_body = response->getResponseData();
	string header = Global::toString(_header);
	string body = Global::toString(_body);
	Document d;
	d.Parse<0>(body.c_str());
	if (d.HasParseError())
		CCLOG("GetParseError %s\n", d.GetParseError());

	if (d["result"].GetBool()) {
		string result = d["info"].GetString();
		for (int i = 0; i < result.length(); i++)
			if (result[i] == '|')
				result[i] = '\n';
		rank_field->setText(result);
	}
	else
		MessageBox(d["info"].GetString(), "Get Rank Failed");
}
