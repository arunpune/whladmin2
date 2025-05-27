$(function () {

  // Disable submission
  $('.whl-action-change-password').attr('disabled', 'disabled');

  $('.whl-formfield-currentpassword').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidPassword(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleChangePassword();
  });
  $('.whl-formfield-password').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v1 = $('.whl-formfield-currentpassword').val();
    var v = $(this).val();
    if (fnIsValidPassword(v) && v1 !== v) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleChangePassword();
  });
  $('.whl-formfield-confirmpassword').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v1 = $('.whl-formfield-password').val();
    var v = $(this).val();
    if (fnIsValidPassword(v) && v1 === v) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleChangePassword();
  });

  function fnToggleChangePassword() {
    var opw = $('.whl-formfield-currentpassword').val();
    var pw = $('.whl-formfield-password').val();
    var cpw = $('.whl-formfield-confirmpassword').val();
    var allowSubmission = opw !== undefined && opw !== null && opw.trim().length >= 14
      && pw !== undefined && pw !== null && pw.trim().length >= 14
      && cpw !== undefined && cpw !== null && cpw.trim().length >= 14
      && opw != pw
      && pw === cpw;
    if (allowSubmission) {
      $('.whl-action-change-password').removeAttr('disabled');
    }
    else {
      $('.whl-action-change-password').attr('disabled', 'disabled');
    }
  }

});