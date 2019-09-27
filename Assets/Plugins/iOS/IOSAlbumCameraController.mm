//
//  OpenPhotoController.m
//
//  Created by AnYuanLzh
//
 
#import "IOSAlbumCameraController.h"
 
@implementation IOSAlbumCameraController
 
- (void)showActionSheet
{
    NSLog(@" --- showActionSheet !!");
    
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:nil message:nil preferredStyle:UIAlertControllerStyleActionSheet];
    
    UIAlertAction *albumAction = [UIAlertAction actionWithTitle:@"相册" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click album action!");
        [self showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:YES];
    }];
    
    UIAlertAction *cameraAction = [UIAlertAction actionWithTitle:@"相机" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click camera action!");
        [self showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:YES];
    }];
    
    UIAlertAction *album_cameraAction = [UIAlertAction actionWithTitle:@"相册&相机" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click album&camera action!");
        //[self showPicker:UIImagePickerControllerSourceTypeCamera];
        [self showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:YES];
    }];
    
    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"取消" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"click cancel action!");
    }];
    
    
    [alertController addAction:albumAction];
    [alertController addAction:cameraAction];
    [alertController addAction:album_cameraAction];
    [alertController addAction:cancelAction];
    
    UIViewController *vc = UnityGetGLViewController();
    [vc presentViewController:alertController animated:YES completion:^{
        NSLog(@"showActionSheet -- completion");
    }];
}
 
- (void)showPicker:
(UIImagePickerControllerSourceType)type
     allowsEditing:(BOOL)flag
{
    NSLog(@" --- showPicker!!");
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = type;
    picker.allowsEditing = flag;
    
    [self presentViewController:picker animated:YES completion:nil];
}
 
// 打开相册后选择照片时的响应方法
- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
    NSLog(@" --- imagePickerController didFinishPickingMediaWithInfo!!");
    // Grab the image and write it to disk
    UIImage *image;
    UIImage *image2;
    image = [info objectForKey:UIImagePickerControllerEditedImage];
    UIGraphicsBeginImageContext(CGSizeMake(256,256));
    [image drawInRect:CGRectMake(0, 0, 256, 256)];
    image2 = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    // 得到了image，然后用你的函数传回u3d
    NSData *imgData;
    if(UIImagePNGRepresentation(image2) == nil)
    {
        NSLog(@" --- actionSheet slse!! 11 ");
        imgData= UIImageJPEGRepresentation(image, .6);
    }
    else
    {
        NSLog(@" --- actionSheet slse!! 22 ");
        imgData= UIImagePNGRepresentation(image2);
    }
    
    NSString *_encodeImageStr = [imgData base64Encoding];
    UnitySendMessage( "_IOSAlbumManager", "PickImageCallBack_Base64", _encodeImageStr.UTF8String);
    
    // 关闭相册
    [picker dismissViewControllerAnimated:YES completion:nil];
}
 
// 打开相册后点击“取消”的响应
- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    NSLog(@" --- imagePickerControllerDidCancel !!");
    [self dismissViewControllerAnimated:YES completion:nil];
}
 
+(void) saveImageToPhotosAlbum:(NSString*) readAdr
{
    NSLog(@"readAdr: ");
    NSLog(readAdr);
    UIImage* image = [UIImage imageWithContentsOfFile:readAdr];
    UIImageWriteToSavedPhotosAlbum(image,
                                   self,
                                   @selector(image:didFinishSavingWithError:contextInfo:),
                                   NULL);
}
 
+(void) image:(UIImage*)image didFinishSavingWithError:(NSError*)error contextInfo:(void*)contextInfo
{
    NSString* result;
    if(error)
    {
        result = @"图片保存到相册失败!";
    }
    else
    {
        result = @"图片保存到相册成功!";
    }
    UnitySendMessage( "_IOSAlbumManager", "SaveImageToPhotosAlbumCallBack", result.UTF8String);
}
 
@end
 
//------------- called by unity -----begin-----------------
#if defined (__cplusplus)
extern "C" {
#endif
    
    // 弹出一个菜单项：相册、相机
    void _showActionSheet()
    {
        NSLog(@" -unity call-- _showActionSheet !!");
        IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
        UIViewController *vc = UnityGetGLViewController();
        [vc.view addSubview: app.view];
        
        [app showActionSheet];
    }
    
    // 打开相册
    void _iosOpenPhotoLibrary()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:NO];
        }
        else
        {
            UnitySendMessage( "_IOSAlbumManager", "PickImageCallBack_Base64", (@"").UTF8String);
        }
    }
    
    // 打开相册
    void _iosOpenPhotoAlbums()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:NO];
        }
        else
        {
            _iosOpenPhotoLibrary();
        }
    }
    
    // 打开相机
    void _iosOpenCamera()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:NO];
        }
        else
        {
            UnitySendMessage( "_IOSAlbumManager", "PickImageCallBack_Base64", (@"").UTF8String);
        }
    }
    
    
    // 打开相册--可编辑
    void _iosOpenPhotoLibrary_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypePhotoLibrary allowsEditing:YES];
        }
        else
        {
            UnitySendMessage( "_IOSAlbumManager", "PickImageCallBack_Base64", (@"").UTF8String);
        }
        
    }
    
    // 打开相册--可编辑
    void _iosOpenPhotoAlbums_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeSavedPhotosAlbum])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeSavedPhotosAlbum allowsEditing:YES];
        }
        else
        {
            _iosOpenPhotoLibrary();
        }
        
    }
    
    // 打开相机--可编辑
    void _iosOpenCamera_allowsEditing()
    {
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            IOSAlbumCameraController * app = [[IOSAlbumCameraController alloc] init];
            UIViewController *vc = UnityGetGLViewController();
            [vc.view addSubview: app.view];
            
            [app showPicker:UIImagePickerControllerSourceTypeCamera allowsEditing:YES];
        }
        else
        {
            UnitySendMessage( "_IOSAlbumManager", "PickImageCallBack_Base64", (@"").UTF8String);
        }
    }
	
    void _iosSaveImageToPhotosAlbum(char* readAddr)
    {
        NSString* temp = [NSString stringWithUTF8String:readAddr];
        [IOSAlbumCameraController saveImageToPhotosAlbum:temp];
    }
    
#if defined (__cplusplus)
}
#endif
//------------- called by unity -----end-----------------
