~~~C++
#include <GL/freeglut.h>
#include <stdio.h>
#include <math.h>

GLint winWidth = 500, winHeight = 500; 	      // 设置初始化窗口大小

/* 观察坐标系参数设置 */
GLfloat x0 = 0.0, yy = 2.0, z0 = 5.0;	   // 设置观察坐标系原点 
GLfloat xref = 0.0, yref = 0.0, zref = 0.0;	// 设置观察坐标系参考点（视点） 
GLfloat Vx = 0.0, Vy = 1.0, Vz = 0.0;	   // 设置观察坐标系向上向量（y轴） 

/* 观察体参数设置 */
GLfloat xwMin = -1.0, ywMin = -1.0, xwMax = 1.0, ywMax = 1.0; // 设置裁剪窗口坐标范围
GLfloat dnear = 1.5, dfar = 20.0;	      // 设置远、近裁剪面深度范围

//矩阵乘法函数
void matrixMultiply(GLfloat result[16], const GLfloat a[16], const GLfloat b[16]) {
    for (int i = 0; i < 4; ++i) {
        for (int j = 0; j < 4; ++j) {
            result[i * 4 + j] = 0.0;
            for (int k = 0; k < 4; ++k) {
                result[i * 4 + j] += a[i * 4 + k] * b[k * 4 + j];
            }
        }
    }
}

//生成绕 X 轴旋转的矩阵
void rotateXMatrix(GLfloat matrix[16], float angle)
{
    float rad = angle * 3.1415926 / 180.0;
    matrix[0] = 1.0; matrix[1] = 0.0; matrix[2] = 0.0; matrix[3] = 0.0;
    matrix[4] = 0.0; matrix[5] = cos(rad); matrix[6] = -sin(rad); matrix[7] = 0.0;
    matrix[8] = 0.0; matrix[9] = sin(rad); matrix[10] = cos(rad); matrix[11] = 0.0;
    matrix[12] = 0.0; matrix[13] = 0.0; matrix[14] = 0.0; matrix[15] = 1.0;
}

void init(void)
{
    glClearColor(0.0, 0.0, 0.0, 0.0);
}

float angle = 0.0f;
float step = 0.1f;
float rightForearmAngle = 0.0f;
float step2 = 0.05f;
float leftForearmAngle = 0.0f;
float step3 = -0.05f;
float rightCalfAngle = 0.0f;
float step4 = 0.05f;
float leftCalfAngle = 0.0f;
float step5 = -0.05f;
float cameraAngle = 0.0f;
float cameraDistance = 5.0f;
float cameraStep = 0.01f;

static void idle(void) 
{
    angle += step;
    if(angle > 30.0 || angle < -30.0) 
    {
        step *= -1;
    }
	rightForearmAngle += step2;
	if(rightForearmAngle > 25.0 || rightForearmAngle < -5.0) 
    {
		step2 *= -1;
	}
	leftForearmAngle += step3;
	if(leftForearmAngle > 25.0 || leftForearmAngle < -5.0)
	{       
		step3 *= -1;
	}
	rightCalfAngle += step4;
    if(rightCalfAngle > 0.0 || rightCalfAngle < -30.0)
    {
        step4 *= -1;
    }
    leftCalfAngle += step5;
    if (leftCalfAngle > 0.0 || leftCalfAngle < -30.0)
    {
        step5 *= -1;
    }
	cameraAngle += cameraStep;
	x0 = cameraDistance * sin(cameraAngle * 3.1415926 / 180.0);
	z0 = cameraDistance * cos(cameraAngle * 3.1415926 / 180.0);
	if (cameraAngle > 30.0 || cameraAngle < 0.0)
	{
		cameraStep *= -1;
	}
    glutPostRedisplay();  //标记当前窗口需要重新绘制
}

void display(void) {
    glClear(GL_COLOR_BUFFER_BIT);

    glLoadIdentity();
    /* 观察变换 */
    gluLookAt(x0, yy, z0, xref, yref, zref, Vx, Vy, Vz);        // 指定三维观察参数

    GLfloat rotMatrix[16];
    GLUquadricObj* sphere;

    //脖子
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(0.0f, 1.4f, 0.0f);
    glScalef(0.5f, 0.7f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();

    //头
    glPushMatrix();
    glColor3f(1.0f, 0.5f, 0.2f);
    glTranslatef(0.0f, 1.9f, 0.0f);
    glScalef(1.5f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //左眼
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(-0.17f, 2.05f, 0.11f);
    glScalef(0.3f, 0.1f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //右眼
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(0.17f, 2.05f, 0.11f);
    glScalef(0.3f, 0.1f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //嘴
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glScalef(0.1f, 0.1f, 0.1f);
    glTranslatef(0.0f, 1.75f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(0.0f, 1.75f, 0.1f);
    glScalef(0.5f, 0.1f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(-0.15f, 1.8f, 0.1f);
    glScalef(0.1f, 0.1f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 1.0f);
    glTranslatef(0.15f, 1.8f, 0.1f);
    glScalef(0.1f, 0.1f, 0.1f);
    glutSolidCube(0.5f);
    glPopMatrix();


    //下半身
    float thighLength = 3.0f * 0.5f;
    float calfLength = 3.0f * 0.5f;
    //左大腿
    glPushMatrix();
    glColor3f(0.5f, 0.5f, 1.5f);
    glTranslatef(-0.5f, -1.5f + thighLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, angle);
    glMultMatrixf(rotMatrix);
    glTranslatef(0.0f, -thighLength / 4, 0.0f);
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //左小腿
    glPushMatrix();
    glColor3f(0.5f, 0.5f, 1.5f);
    //继承大腿的变换
    glTranslatef(-0.5f, -1.5f + thighLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, angle);
    glMultMatrixf(rotMatrix);
    //应用小腿自身旋转
    glTranslatef(0.0f, -thighLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, leftCalfAngle);
    glMultMatrixf(rotMatrix);
    //调整小腿位置并绘制
    glTranslatef(0.0f, -calfLength / 4, 0.0f);
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //右大腿
    glPushMatrix();
    glColor3f(0.5f, 0.5f, 1.5f);
    glTranslatef(0.5f, -1.5f + thighLength / 2, 0.0f); 
    rotateXMatrix(rotMatrix, -angle);
    glMultMatrixf(rotMatrix);
    glTranslatef(0.0f, -thighLength / 4, 0.0f);  
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //右小腿
    glPushMatrix();
    glColor3f(0.5f, 0.5f, 1.5f);
    //继承大腿的变换
    glTranslatef(0.5f, -1.5f + thighLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, -angle);
    glMultMatrixf(rotMatrix);
    //应用小腿自身旋转
    glTranslatef(0.0f, -thighLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, rightCalfAngle);
    glMultMatrixf(rotMatrix);
    //调整小腿位置并绘制
    glTranslatef(0.0f, -calfLength / 4, 0.0f);
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();


    //上半身
    float bodyTopY = 0.25f + 4.0f * 0.5f / 2; //计算身体上端的 Y 坐标
    float armLength = 3.0f * 0.5f;
    //左大臂
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 0.0f);
    glTranslatef(-1.25f, bodyTopY, 0.0f); //平移到身体上端位置
    rotateXMatrix(rotMatrix, -angle);
    glMultMatrixf(rotMatrix);
    glTranslatef(0.0f, -armLength / 4, 0.0f); //平移到手臂中心位置
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //左小臂
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 0.0f);
    //应用大臂的旋转，保证小臂跟随大臂旋转
    glTranslatef(-1.25f, bodyTopY, 0.0f);
    rotateXMatrix(rotMatrix, -angle);
    glMultMatrixf(rotMatrix);
    //应用小臂自身的旋转
    glTranslatef(0.0f, -armLength / 2, 0.0f);
    rotateXMatrix(rotMatrix, leftForearmAngle);
    glMultMatrixf(rotMatrix);
    //平移到小臂中心位置
    glTranslatef(0.0f, -armLength / 4, 0.0f);
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    //左手
    glPushMatrix();
    sphere = gluNewQuadric();
    glColor3f(1.0f, 0.5f, 0.2f);
    glTranslatef(0.0f, -armLength / 4, 0.0f);
    glScalef(1.0f, 1.0f, 2.0f);
    gluSphere(sphere, 0.25, 50, 50);
    glPopMatrix();
    glPopMatrix();

    //身体
    glPushMatrix();
    glColor3f(1.0f, 0.0f, 0.0f);
    glTranslatef(0.0f, 0.25f, 0.0f);
    glScalef(4.0f, 4.0f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();

    //右大臂
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 0.0f);
    glTranslatef(1.25f, bodyTopY, 0.0f); //平移到身体上端位置
    rotateXMatrix(rotMatrix, angle);
    glMultMatrixf(rotMatrix);
    glTranslatef(0.0f, -armLength / 4, 0.0f); //平移到手臂中心位置
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    glPopMatrix();
    //右小臂
    glPushMatrix();
    glColor3f(1.0f, 1.0f, 0.0f);
    //应用大臂的旋转，保证小臂跟随大臂旋转
    glTranslatef(1.25f, bodyTopY, 0.0f);
    rotateXMatrix(rotMatrix, angle);
    glMultMatrixf(rotMatrix);
    //应用小臂自身的旋转
    glTranslatef(0.0f, -armLength / 2, 0.0f); 
    rotateXMatrix(rotMatrix, rightForearmAngle);
    glMultMatrixf(rotMatrix);
    //平移到小臂中心位置
    glTranslatef(0.0f, -armLength / 4, 0.0f); 
    glScalef(1.0f, 1.5f, 0.5f);
    glutSolidCube(0.5f);
    //右手
    glPushMatrix();
    sphere = gluNewQuadric();
    glColor3f(1.0f, 0.5f, 0.2f);
    glTranslatef(0.0f, -armLength / 4, 0.0f);
    glScalef(1.0f, 1.0f, 2.0f);
    gluSphere(sphere, 0.25, 50, 50);
    glPopMatrix();
    glPopMatrix();


    glFlush();
}

void reshape(GLint newWidth, GLint newHeight) {
    /* 视口变换 */
    glViewport(0, 0, newWidth, newHeight);	// 定义视口大小

    /* 投影变换 */
    glMatrixMode(GL_PROJECTION);

    glLoadIdentity();

    /* 透视投影，设置透视观察体 */
    glFrustum(xwMin, xwMax, ywMin, ywMax, dnear, dfar);

    /* 模型变换 */
    glMatrixMode(GL_MODELVIEW);

    winWidth = newWidth;
    winHeight = newHeight;
}

int main(int argc, char* argv[]) {
    glutInit(&argc, argv);
    glutInitWindowPosition(100, 100);
    glutInitWindowSize(500, 500);        // 设置初始化窗口大小
    glutCreateWindow("三维观察");
    init();
    glutDisplayFunc(display);
    glutReshapeFunc(reshape);
    glutIdleFunc(idle);
    glutMainLoop();

    return 0;
}
~~~

