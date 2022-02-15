#ifndef TEST_BUFFER11
#define TEST_BUFFER11
StructuredBuffer<float> volume;

void MyFunctionA_float(float i,out float v)
{
    v=volume[(int)i];
}
#endif 