$(function () {
  $('.whl-action-register').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-register').attr('disabled', 'disabled');
    $('.whl-action-register').html('Registering...');
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
            $('.whl-form-register').submit();
          });
      });
    } else {
      $('.whl-form-register').submit();
    }
  });
});
function fnRegister(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-register').submit();
}