$(function () {

  // Disable submission
  $('.whl-action-reset-password').attr('disabled', 'disabled');

  $('.whl-formfield-password').on('change', function (e) {
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
    var pw = $('.whl-formfield-password').val();
    var cpw = $('.whl-formfield-confirmpassword').val();
    var allowSubmission = fnIsValidPassword(pw)
      && fnIsValidPassword(cpw)
      && pw === cpw;
    if (allowSubmission) {
      $('.whl-action-reset-password').removeAttr('disabled');
    }
    else {
      $('.whl-action-reset-password').attr('disabled', 'disabled');
    }
  }

});