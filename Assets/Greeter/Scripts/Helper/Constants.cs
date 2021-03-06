﻿
public static class Constants
{
    public const string PREFIX_TRAIN_IMAGES_PATH = "/Images/People/";
    public const string PREFIX_DETECTION_IMAGES_PATH = "/Images/Runtime/";
    public const string PREFIX_TRAIN_IMAGE_NAME = "people";
    public const string IMAGE_FORMAT = ".jpg";

    // UI Text
    public const string STATUS_PREFIX = "処理結果：";
    public const string ERROR_PEOPLE_GROUP_CANNOT_CREATE = "Cannot create PeopleGroup";

    // Must be followed by /year/month/day to get today's horoscopes
    public const string ZODIAC_API_ENDPOINT_PREFIX = "http://api.jugemkey.jp/api/horoscope/free";
}
