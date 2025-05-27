$(function () {
  $('.whl-action-login').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-login').attr('disabled', 'disabled');
    $('.whl-action-login').html('Signing in...');
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
            $('.whl-form-login').submit();
          });
      });
    } else {
      $('.whl-form-login').submit();
    }
  });
});
function fnLogin(t) {
  console.debug('recaptcha token: ' + t);
  $('.whl-formfield-grecaptchatoken').val(t);
  $('.whl-form-login').submit();
}