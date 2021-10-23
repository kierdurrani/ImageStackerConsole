// MathLibrary.h - Contains declarations of math functions
#pragma once

#ifdef StackerDLLs
#define StackerDLLs __declspec(dllexport)
#else
#define StackerDLLs __declspec(dllimport)
#endif

// The Fibonacci recurrence relation describes a sequence F
// where F(n) is { n = 0, a
//               { n = 1, b


extern "C" StackerDLLs int sum_values(int* array1, int size);
