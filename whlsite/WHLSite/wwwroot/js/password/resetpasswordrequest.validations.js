$(function () {
  // Disable submission
  $('.whl-action-reset-password-request').attr('disabled', 'disabled');

  $('.whl-formfield-username').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidUsername(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRequest();
  });

  function fnToggleRequest() {
    var un = $('.whl-formfield-username').val();
    var allowSubmission = fnIsValidUsername(un);
    if (allowSubmission) {
      $('.whl-action-reset-password-request').removeAttr('disabled');
    }
    else {
      $('.whl-action-reset-password-request').attr('disabled', 'disabled');
    }
  }

});