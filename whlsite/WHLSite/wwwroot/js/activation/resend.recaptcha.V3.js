$(function() {
  $('.whl-action-resend-activation').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-resend-activation').attr('disabled', 'disabled');
    $('.whl-action-resend-activation').html('Requesting...');
    var re = $('.whl-formfield-recaptchaenabled').val();
    console.log(re);
    if (re.toLowerCase() === '1') {
      grecaptcha.enterprise.ready(async () => {
        const k = $('.whl-formfield-recaptchakey').val();
        const a = $('.whl-formfield-recaptchaaction').val();
        grecaptcha.enterprise.execute(k, { action: a })
          .then(function (t) {
            console.debug('recaptcha token: ' + t);
            $('.whl-formfield-grecaptchatoken').val(t);
            $('.whl-resend-activation').submit();
          });
      });
    } else {
      $('.whl-form-resend-activation').submit();
    }
  });
});
function fnResendActivation(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-resend-activation').submit();
}