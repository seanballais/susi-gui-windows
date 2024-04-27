#include <codecvt>
#include <format>
#include <locale>
#include <string>

#pragma once
#include "ffi.hpp"

void initLogging() {
	init_logging();
}

template<class... Args>
void logInfo(std::wformat_string<Args...> fmt, Args&&... args)
{
	std::wstring message;
	auto it = std::back_inserter(message);
	std::format_to(it, fmt, std::forward<Args>(args)...);

	// Based on: https://stackoverflow.com/a/59617138/1116098
	int count = WideCharToMultiByte(CP_UTF8, 0, message.c_str(), message.length(), NULL, 0, NULL, NULL);
	std::string message_utf8(count, 0);
	WideCharToMultiByte(CP_UTF8, 0, message.c_str(), -1, &message_utf8[0], count, NULL, NULL);
	
	log_info(message_utf8.c_str());
}
