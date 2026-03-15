// License Key Generator
//
// Generates and validates license keys based on the machine's MAC address.
// The key is derived by transforming MAC bytes with XOR scrambling and
// formatting into a human-readable XXXXX-XXXXX-XXXXX format.
//
// Build: g++ -std=c++17 -o keygen keygen.cpp && ./keygen

#include <iostream>
#include <string>
#include <sstream>
#include <iomanip>
#include <cstdint>
#include <cstdlib>
#include <array>

#ifdef __APPLE__
#include <ifaddrs.h>
#include <net/if.h>
#include <net/if_dl.h>
#include <net/if_types.h>
#elif __linux__
#include <fstream>
#include <net/if.h>
#endif

// ── MAC Address ────────────────────────────────────────

std::string get_mac_address() {
#ifdef __APPLE__
    struct ifaddrs* iflist;
    if (getifaddrs(&iflist) != 0) return "";

    std::string mac;
    for (struct ifaddrs* cur = iflist; cur; cur = cur->ifa_next) {
        if (cur->ifa_addr->sa_family != AF_LINK) continue;
        auto* sdl = reinterpret_cast<struct sockaddr_dl*>(cur->ifa_addr);
        if (sdl->sdl_type != IFT_ETHER) continue;

        auto* ptr = reinterpret_cast<unsigned char*>(LLADDR(sdl));
        std::ostringstream ss;
        for (int i = 0; i < 6; i++)
            ss << std::hex << std::uppercase << std::setfill('0') << std::setw(2) << (int)ptr[i];
        mac = ss.str();
        break;
    }
    freeifaddrs(iflist);
    return mac;

#elif __linux__
    std::ifstream f("/sys/class/net/eth0/address");
    if (!f.is_open()) f.open("/sys/class/net/enp0s3/address");
    if (!f.is_open()) return "";
    std::string line;
    std::getline(f, line);
    std::string mac;
    for (char c : line)
        if (c != ':') mac += std::toupper(c);
    return mac;

#else
    return "";
#endif
}

// ── Key Generation ─────────────────────────────────────

std::string generate_key(const std::string& mac) {
    if (mac.size() < 12) return "";

    // XOR-scramble MAC bytes with a fixed seed to make the key non-obvious
    const uint8_t seed[] = {0x4B, 0x65, 0x79, 0x47, 0x65, 0x6E};  // "KeyGen"
    std::array<uint8_t, 6> bytes{};
    for (int i = 0; i < 6; i++) {
        uint8_t hi = (mac[i * 2] >= 'A') ? (mac[i * 2] - 'A' + 10) : (mac[i * 2] - '0');
        uint8_t lo = (mac[i * 2 + 1] >= 'A') ? (mac[i * 2 + 1] - 'A' + 10) : (mac[i * 2 + 1] - '0');
        bytes[i] = ((hi << 4) | lo) ^ seed[i];
    }

    // Expand 6 bytes into 15-char key (3 groups of 5)
    // Use base-36 encoding (0-9, A-Z)
    const char* b36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    std::string key;
    for (int group = 0; group < 3; group++) {
        uint16_t val = (bytes[group * 2] << 8) | bytes[group * 2 + 1];
        for (int i = 0; i < 5; i++) {
            key += b36[val % 36];
            val /= 36;
        }
        if (group < 2) key += '-';
    }
    return key;
}

bool validate_key(const std::string& key, const std::string& mac) {
    return !key.empty() && key == generate_key(mac);
}

// ── UI ─────────────────────────────────────────────────

void print_header() {
    std::cout << R"(
  ╔═══════════════════════════════════════╗
  ║         LICENSE KEY GENERATOR         ║
  ╚═══════════════════════════════════════╝
)" << std::endl;
}

void print_menu() {
    std::cout << "  [1] Generate key for this machine\n"
              << "  [2] Validate a key\n"
              << "  [3] Show MAC address\n"
              << "  [0] Exit\n"
              << "\n  > ";
}

int main() {
    std::string mac = get_mac_address();

    print_header();

    if (mac.empty()) {
        std::cout << "  Warning: could not detect MAC address.\n\n";
    }

    while (true) {
        print_menu();

        std::string choice;
        std::getline(std::cin, choice);
        std::cout << std::endl;

        if (choice == "1") {
            if (mac.empty()) {
                std::cout << "  Error: no MAC address available.\n\n";
                continue;
            }
            std::string key = generate_key(mac);
            std::cout << "  Your license key:\n\n"
                      << "    ┌─────────────────────┐\n"
                      << "    │  " << key << "  │\n"
                      << "    └─────────────────────┘\n\n";

        } else if (choice == "2") {
            std::cout << "  Enter key: ";
            std::string input;
            std::getline(std::cin, input);

            if (validate_key(input, mac)) {
                std::cout << "\n  ✓ Key is VALID for this machine.\n\n";
            } else {
                std::cout << "\n  ✗ Key is INVALID.\n\n";
            }

        } else if (choice == "3") {
            if (mac.empty())
                std::cout << "  MAC: not available\n\n";
            else
                std::cout << "  MAC: " << mac << "\n\n";

        } else if (choice == "0") {
            std::cout << "  Bye.\n";
            break;
        }
    }

    return 0;
}
