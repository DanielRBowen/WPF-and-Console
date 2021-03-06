// MostCommonWordsInTextFileC++.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

class Stopwatch
{
private:
	long long myStartedAt;

public:
	static Stopwatch StartNew()
	{
		auto stopwatch = Stopwatch();
		stopwatch.Start();
		return stopwatch;
	}

	double GetElapsedSeconds()
	{
		long long finishedAt, frequency;
		QueryPerformanceCounter((LARGE_INTEGER*)&finishedAt);
		QueryPerformanceFrequency((LARGE_INTEGER*)&frequency);
		return (double)(finishedAt - myStartedAt) / (double)frequency;
	}

	void Start()
	{
		QueryPerformanceCounter((LARGE_INTEGER*)&myStartedAt);
		return;
	}
};

wstring ToCurrentCultureString(const wstring &number, const NUMBERFMTW* format)
{
	auto characterCount = GetNumberFormatEx(LOCALE_NAME_USER_DEFAULT, 0, number.c_str(), format, nullptr, 0);

	if (!characterCount)
	{
		return number;
	}

	vector<wchar_t> currentCultureString(characterCount);
	GetNumberFormatEx(LOCALE_NAME_USER_DEFAULT, 0, number.c_str(), format, &currentCultureString[0], characterCount);

	return wstring(currentCultureString.begin(), currentCultureString.end());
}

void GetDefaultNumberFormat(NUMBERFMTW &format)
{
	GetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_IDIGITS | LOCALE_RETURN_NUMBER, (LPWSTR)&format.NumDigits, sizeof(format.NumDigits) / sizeof(wchar_t));
	GetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_ILZERO | LOCALE_RETURN_NUMBER, (LPWSTR)&format.LeadingZero, sizeof(format.LeadingZero) / sizeof(wchar_t));

	wchar_t grouping[32] = L"";
	GetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_SGROUPING, grouping, ARRAYSIZE(grouping));

	if (lstrcmp(grouping, L"3;0") == 0)
	{
		format.Grouping = 3;
	}
	else
	{
		format.Grouping = 0;
	}

	GetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_SDECIMAL, format.lpDecimalSep, 16);
	GetLocaleInfoEx(LOCALE_NAME_USER_DEFAULT, LOCALE_STHOUSAND, format.lpThousandSep, 16);

	GetLocaleInfoEx
	(
		LOCALE_NAME_USER_DEFAULT,
		LOCALE_INEGNUMBER | LOCALE_RETURN_NUMBER,
		(LPWSTR)&format.NegativeOrder,
		sizeof(format.NegativeOrder) / sizeof(wchar_t)
	);

	return;
}

wstring ToCurrentCultureString(unsigned int number)
{
	wstringstream stream;
	stream << number;

	NUMBERFMTW format;

	wchar_t decimalSeparator[16] = L"";
	format.lpDecimalSep = decimalSeparator;

	wchar_t thousandSeparator[16] = L"";
	format.lpThousandSep = thousandSeparator;

	GetDefaultNumberFormat(format);
	format.NumDigits = 0;

	return ToCurrentCultureString(stream.str(), &format);
}

int _tmain(int argc, _TCHAR* argv[])
{
	auto stopwatch = Stopwatch::StartNew();

	ifstream inputStream(L"../MostCommonWordsInTextFile/ThePictureOfDorianGray.txt");

	if (!inputStream.is_open())
	{
		cout << "file not found.";
		return -1;
	}

	stringstream buffer;
	buffer << inputStream.rdbuf();
	inputStream.close();
	auto text = buffer.str();

	auto begin = text.begin();
	transform(begin, text.end(), begin, tolower);

	unordered_map<string, unsigned int> wordCounts;
	istringstream textStream(text);

	while (textStream)
	{
		string word;
		textStream >> word;

		if (word.length() == 0)
		{
			continue;
		}

		unsigned int count;

		if (wordCounts.count(word))
		{
			count = wordCounts[word];
		}
		else
		{
			count = 0;
		}

		wordCounts[word] = count + 1;
	}

	vector<pair<string, unsigned int>> wordCountsVector(wordCounts.bucket_count());
	copy(wordCounts.cbegin(), wordCounts.cend(), back_inserter(wordCountsVector));

	sort(wordCountsVector.begin(), wordCountsVector.end(), [](const pair<string, unsigned int> &left, const pair<string, unsigned int> &right)
	{
		return left.second>right.second;
	});

	for (unsigned int index = 0, count = min(wordCountsVector.size(), 10U); index != count; ++index)
	{
		const auto &word = wordCountsVector[index];
		cout << word.first;
		cout << '\t';
		wcout << ToCurrentCultureString(word.second) << endl;
	}

	wcout << stopwatch.GetElapsedSeconds() << endl;

	return 0;
}
