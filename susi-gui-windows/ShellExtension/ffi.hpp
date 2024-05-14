extern "C" {
	__declspec(dllimport) void init_logging();
	__declspec(dllimport) void log_info(const char*);
}
