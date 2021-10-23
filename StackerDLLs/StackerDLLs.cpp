#include "StackerDLLs.h"

int sum_values(int array1[], int size) {
	

	int sum = 0; 
	for (int i = 0; i < size; i++) {

		sum += array1[i] * array1[i];
	}

	return sum;
}
