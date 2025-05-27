$(function() {

  $('.whl-action-save-networth').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-networth').hide();
    $('.whl-action-save-networth').attr('disabled', 'disabled');
    $('.whl-action-save-networth').html('Saving...');
    $('.whl-form-edit-networth').submit();
  });

});