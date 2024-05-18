// Susi
// Copyright (C) 2024  Sean Francis N.Ballais
//
// This program is free software : you can redistribute it and /or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see < http://www.gnu.org/licenses/>.
#pragma once

#include <codecvt>
#include <format>
#include <locale>
#include <string>

#include <Windows.h>

#include "ffi.hpp"

void initLogging();

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
