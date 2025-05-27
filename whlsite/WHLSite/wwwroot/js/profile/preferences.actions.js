$(function() {

  $('.whl-action-save-preferences').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-preferences').hide();
    $('.whl-action-save-preferences').attr('disabled', 'disabled');
    $('.whl-action-save-preferences').html('Saving...');
    $('.whl-form-edit-preferences').submit();
  });

});