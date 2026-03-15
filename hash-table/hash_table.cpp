// Hash Table with Separate Chaining
//
// A generic hash table implementation using linked-list buckets
// and a polynomial rolling hash function.
//
// Build: g++ -std=c++17 -o hash_table hash_table.cpp && ./hash_table

#include <iostream>
#include <string>
#include <functional>
#include <chrono>

// ── Linked List (bucket chain) ─────────────────────────

template <typename T>
struct Node {
    T data;
    Node* next;
    Node(const T& d, Node* n = nullptr) : data(d), next(n) {}
};

template <typename T>
class List {
    Node<T>* head = nullptr;
    int size_ = 0;

public:
    ~List() {
        while (head) {
            Node<T>* tmp = head;
            head = head->next;
            delete tmp;
        }
    }

    void add(const T& elem) {
        head = new Node<T>(elem, head);
        size_++;
    }

    T* find(const T& elem) {
        for (Node<T>* p = head; p; p = p->next)
            if (p->data == elem)
                return &p->data;
        return nullptr;
    }

    bool remove(const T& elem) {
        Node<T>* prev = nullptr;
        for (Node<T>* p = head; p; prev = p, p = p->next) {
            if (p->data == elem) {
                if (prev) prev->next = p->next;
                else head = p->next;
                delete p;
                size_--;
                return true;
            }
        }
        return false;
    }

    void print() const {
        for (Node<T>* p = head; p; p = p->next)
            std::cout << "    " << p->data << "\n";
    }

    int size() const { return size_; }
    bool empty() const { return size_ == 0; }
};

// ── Hash Table ─────────────────────────────────────────

template <typename T>
class HashTable {
    static constexpr int BUCKET_COUNT = 17;
    List<T> buckets[BUCKET_COUNT];
    std::function<int(const T&)> hash_fn;
    int count_ = 0;

public:
    HashTable(std::function<int(const T&)> fn) : hash_fn(fn) {}

    void add(const T& elem) {
        int idx = hash_fn(elem) % BUCKET_COUNT;
        buckets[idx].add(elem);
        count_++;
    }

    T* find(const T& elem) {
        int idx = hash_fn(elem) % BUCKET_COUNT;
        return buckets[idx].find(elem);
    }

    bool remove(const T& elem) {
        int idx = hash_fn(elem) % BUCKET_COUNT;
        if (buckets[idx].remove(elem)) {
            count_--;
            return true;
        }
        return false;
    }

    void print() const {
        for (int i = 0; i < BUCKET_COUNT; i++) {
            if (!buckets[i].empty()) {
                std::cout << "  bucket[" << i << "]:\n";
                buckets[i].print();
            }
        }
    }

    int size() const { return count_; }
};

// ── Demo: Human struct ─────────────────────────────────

struct Human {
    std::string name;
    std::string surname;

    bool operator==(const Human& other) const {
        return name == other.name && surname == other.surname;
    }

    friend std::ostream& operator<<(std::ostream& out, const Human& h) {
        return out << h.name << " " << h.surname;
    }
};

// Polynomial rolling hash (base 53)
int human_hash(const Human& h) {
    std::string str = h.name + h.surname;
    const int p = 53;
    long long hash = 0, p_pow = 1;
    for (char c : str) {
        hash += (c - 'a' + 1) * p_pow;
        p_pow *= p;
    }
    return std::abs(static_cast<int>(hash));
}

// ── Main ───────────────────────────────────────────────

int main() {
    HashTable<Human> ht(human_hash);

    Human people[] = {
        {"Kirill",  "Beldiaga"},
        {"Bogdan",  "Kalnybolochyk"},
        {"Taras",   "Shevchenko"},
        {"Ada",     "Lovelace"},
        {"Alan",    "Turing"},
        {"Edsger",  "Dijkstra"},
    };

    std::cout << "=== Hash Table with Separate Chaining ===\n\n";

    // Insert
    std::cout << "Inserting " << std::size(people) << " entries...\n\n";
    auto t0 = std::chrono::high_resolution_clock::now();
    for (const auto& p : people)
        ht.add(p);
    auto t1 = std::chrono::high_resolution_clock::now();
    auto us = std::chrono::duration_cast<std::chrono::microseconds>(t1 - t0).count();

    ht.print();
    std::cout << "\n  Total: " << ht.size() << " entries, insert time: " << us << " us\n";

    // Search
    std::cout << "\nSearching:\n";
    Human query{"Alan", "Turing"};
    Human* found = ht.find(query);
    std::cout << "  find(Alan Turing) -> " << (found ? "FOUND" : "NOT FOUND") << "\n";

    Human missing{"Grace", "Hopper"};
    found = ht.find(missing);
    std::cout << "  find(Grace Hopper) -> " << (found ? "FOUND" : "NOT FOUND") << "\n";

    // Delete
    std::cout << "\nDeleting Edsger Dijkstra...\n";
    ht.remove({"Edsger", "Dijkstra"});
    std::cout << "  Size after delete: " << ht.size() << "\n";

    found = ht.find({"Edsger", "Dijkstra"});
    std::cout << "  find(Edsger Dijkstra) -> " << (found ? "FOUND" : "NOT FOUND") << "\n";

    return 0;
}
