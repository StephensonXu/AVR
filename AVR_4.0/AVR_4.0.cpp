// AVR_4.0.cpp: ����Ŀ�ļ���

#include "stdafx.h"

#using <mscorlib.dll>
using namespace System;

#using "CreateDllDemo.dll" 
using namespace CreateDllDemo;


using namespace System;

int main(array<System::String ^> ^args)
{
	Show ^sh = gcnew Show();
	sh->show();
    Console::WriteLine(L"Hello World");
    return 0;
}
