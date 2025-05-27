$(function() {

  $('.whl-action-save-member').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-member').hide();
    $('.whl-action-save-member').attr('disabled', 'disabled');
    $('.whl-action-save-member').html('Saving...');
    $('.whl-form-edit-member').submit();
  });

});