#include <string>
#include <stdlib.h>
#include <iostream>

class Animal
{
    public:
        virtual void MakeNoise() = 0;
};

class Dog : public Animal
{
    public:
        virtual void MakeNoise() override {
            std::cout << "Woof\n";
        }
};

class Cat : public Animal
{
    public:
        virtual void MakeNoise() override {
            std::cout << "Meaux\n";
        }
};

class Human : public Animal {
    public: 
        Human(std::string name) : name(name) {
        }

    virtual void MakeNoise() override {
        std::cout << "Hello, my name is " << name <<"\n";
    }

    private:
        std::string name;
};


int main(int argc, char ** argv) {
    auto n = atoi(argv[1]);
    std::unique_ptr<Animal> animal;
    switch (n)
    {
        case 1: animal = std::make_unique<Dog>(); break;
        case 2: animal = std::make_unique<Cat>(); break;
        case 3: animal = std::make_unique<Human>(argv[2]); break;
    }
    animal->MakeNoise();
    return 0;
}