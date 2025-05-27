$(function() {

  $('.whl-action-save-account').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-account').hide();
    $('.whl-action-save-account').attr('disabled', 'disabled');
    $('.whl-action-save-account').html('Saving...');
    $('.whl-form-edit-account').submit();
  });

});