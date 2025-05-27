$(function () {

  // Disable submission
  $('.whl-action-login').attr('disabled', 'disabled');

  $('.whl-formfield-username').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidUsername(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleLogin();
  });

  $('.whl-formfield-password').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidPassword(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleLogin();
  });

  function fnToggleLogin() {
    var un = $('.whl-formfield-username').val();
    var pw = $('.whl-formfield-password').val();
    var allowSubmission = fnIsValidUsername(un) && fnIsValidPassword(pw);
    if (allowSubmission) {
      $('.whl-action-login').removeAttr('disabled');
    }
    else {
      $('.whl-action-login').attr('disabled', 'disabled');
    }
  }

});