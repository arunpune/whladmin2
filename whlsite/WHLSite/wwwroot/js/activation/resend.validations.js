$(function () {

  // Disable submission
  $('.whl-action-resend-activation').attr('disabled', 'disabled');

  $('.whl-formfield-username').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidUsername(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleResendActivation();
  });

  function fnToggleResendActivation() {
    var un = $('.whl-formfield-username').val();
    var allowSubmission = fnIsValidUsername(un);
    if (allowSubmission) {
      $('.whl-action-resend-activation').removeAttr('disabled');
    }
    else {
      $('.whl-action-resend-activation').attr('disabled', 'disabled');
    }
  }

});