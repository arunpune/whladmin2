$(function() {

  $('.whl-action-save-profile').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-profile').hide();
    $('.whl-action-save-profile').attr('disabled', 'disabled');
    $('.whl-action-save-profile').html('Saving...');
    $('.whl-form-edit-profile').submit();
  });

});