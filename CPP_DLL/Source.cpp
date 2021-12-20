#include "pch.h"
#include <time.h>
#include "mkl.h"

extern "C"  _declspec(dllexport)
int integrals_computation(MKL_INT nx, MKL_INT ny, float* x, float* y, int nlim, float* left, float* right, float* calculated_integrals, int& ret)
{
	try {
		DFTaskPtr task;
		float* coefs = new float[ny * DF_PP_CUBIC * (nx - 1)];
		float* res = new float[ny * nx];
		ret = dfsNewTask1D(&task, nx, x, DF_UNIFORM_PARTITION, ny, y, DF_MATRIX_STORAGE_ROWS);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		ret = dfsEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_FREE_END, NULL, DF_NO_IC, NULL, coefs, DF_NO_HINT);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		ret = dfsConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		ret = dfsInterpolate1D(task, DF_INTERP, DF_METHOD_PP, nx, x, DF_UNIFORM_PARTITION, 1, new int[1]{ 1 }, NULL, res, DF_MATRIX_STORAGE_ROWS, NULL);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		ret = dfsIntegrate1D(task, DF_METHOD_PP, nlim, left, DF_UNIFORM_PARTITION, right, DF_UNIFORM_PARTITION, NULL, NULL, calculated_integrals, DF_MATRIX_STORAGE_ROWS);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		ret = dfDeleteTask(&task);
		if (ret != DF_STATUS_OK) {
			return 0;
		}
		return 1;
	}
	catch (...) {
		return 0;
	}

}