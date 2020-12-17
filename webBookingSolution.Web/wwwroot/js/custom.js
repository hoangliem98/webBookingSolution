/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
/////////////loading
$(window).on('load', function (event) {
    setTimeout(function () {
        $('body').removeClass('loading-active');
    }, 1000);
});
$(document).ready(function () {
    //////////upload-img
    $('.input-img').change(function () {
        let reader = new FileReader();
        reader.onload = (e) => {
            $file = reader.result;
            if ($file.indexOf('image/jpeg') > 0 || $file.indexOf('image/png') > 0) {
                $('.image_preview_container').attr('src', e.target.result);
            } else {
                alert('Sai định dạng. Chỉ được thêm hình ảnh');
                $('.image_preview_container').attr('src', '/images/preview.jpg');
                $('.image_upload').val('');
                console.log($('.input-img').val());
            }
        }
        reader.readAsDataURL(this.files[0]);
    });
    /////set delay time
    var delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();
    //////minimum && maximum
    $('.maximum').focusout(function () {
        var a = Number($('.maximum').val());
        var b = Number($('.minimum').val());
        if (a < b) {
            alert('Số bạn tối đa không được nhỏ hơn số bàn tối thiểu !!!');
            $('.maximum').val('');
        }
    });

    ////change password
    $(".changepass").change(function () {
        if ($(this).is(":checked"))
        {
            $(".password").removeAttr('readonly');
        } else
        {
            $(".password").attr('readonly', '');
        }
    });
});


    


